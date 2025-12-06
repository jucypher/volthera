using NUnit.Framework;
using UnityEngine;
using TMPro;

public class UIManagerPlayModeTests
{
    private GameObject uiGO;
    private UIManager uiManager;

    private TMP_Text activeText;
    private TMP_Text roundText;
    private TMP_Text winScoreText;
    private TMP_Text winnerText;
    private TMP_InputField inputField;
    private GameObject popup;

    [SetUp]
    public void SetUp()
    {
        uiGO = new GameObject("UIManager");
        uiManager = uiGO.AddComponent<UIManager>();

        activeText = new GameObject("ActiveText").AddComponent<TextMeshProUGUI>();
        roundText = new GameObject("RoundText").AddComponent<TextMeshProUGUI>();
        winScoreText = new GameObject("WinScoreText").AddComponent<TextMeshProUGUI>();
        winnerText = new GameObject("WinnerText").AddComponent<TextMeshProUGUI>();
        inputField = new GameObject("Input").AddComponent<TMP_InputField>();

        popup = new GameObject("SavePopup");
        popup.SetActive(false);

        // 🔥 privát mezők beállítása reflectionnel
        SetPrivate("activePlayerText", activeText);
        SetPrivate("roundText", roundText);
        SetPrivate("winScoreText", winScoreText);
        SetPrivate("winnerText", winnerText);
        SetPrivate("saveNameInput", inputField);
        SetPrivate("savePopup", popup);
    }

    private void SetPrivate(string field, object value)
    {
        typeof(UIManager)
            .GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(uiManager, value);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(uiGO);
    }

    // =============================
    // ✅ TESZTEK
    // =============================

    [Test]
    public void DisplayWinScore_WritesCorrectText()
    {
        uiManager.DisplayWinScore(12);
        Assert.AreEqual("Win at: 12 points", winScoreText.text);
    }

    [Test]
    public void UpdateRound_WritesCorrectText()
    {
        uiManager.UpdateRound(7);
        Assert.AreEqual("Round: 7", roundText.text);
    }

    [Test]
    public void OnSaveButtonClicked_OpensPopupAndClearsInput()
    {
        inputField.text = "MENTES";

        uiManager.OnSaveButtonClicked();

        Assert.IsTrue(popup.activeSelf);
        Assert.AreEqual("", inputField.text);
    }

    [Test]
    public void SetWinner_DisplaysWinnerText()
    {
        GameObject playerGO = new GameObject("Player");
        Player p = playerGO.AddComponent<Player>();
        p.PlayerName = "TESZT_JATEKOS";

        uiManager.SetWinner(p);

        Assert.IsTrue(winnerText.gameObject.activeSelf);
        Assert.AreEqual("TESZT_JATEKOS WINS!", winnerText.text);

        Object.DestroyImmediate(playerGO);
    }
}
