using NUnit.Framework;
using UnityEngine;
using TMPro;

public class UIManagerTests
{
    private GameObject uiGO;
    private UIManager uiManager;

    private TextMeshProUGUI activePlayerText;
    private TextMeshProUGUI[] scoreTexts;
    private TextMeshProUGUI winnerText;

    [SetUp]
    public void Setup()
    {
        // Canvas létrehozása, mert TextMeshProUGUI-hoz kell
        var canvasGO = new GameObject("Canvas", typeof(Canvas));
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        // UIManager létrehozása
        uiGO = new GameObject("UIManagerTest");
        uiGO.transform.SetParent(canvasGO.transform);
        uiManager = uiGO.AddComponent<UIManager>();

        // ActivePlayerText
        var activeGO = new GameObject("ActivePlayerText");
        activeGO.transform.SetParent(uiGO.transform);
        activePlayerText = activeGO.AddComponent<TextMeshProUGUI>();
        uiManager.GetType()
            .GetField("activePlayerText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(uiManager, activePlayerText);

        // ScoreTexts
        scoreTexts = new TextMeshProUGUI[2];
        for (int i = 0; i < 2; i++)
        {
            var scoreGO = new GameObject($"ScoreText{i}");
            scoreGO.transform.SetParent(uiGO.transform);
            scoreTexts[i] = scoreGO.AddComponent<TextMeshProUGUI>();
        }
        uiManager.GetType()
            .GetField("playerScoreTexts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(uiManager, scoreTexts);

        // WinnerText
        var winnerGO = new GameObject("WinnerText");
        winnerGO.transform.SetParent(uiGO.transform);
        winnerText = winnerGO.AddComponent<TextMeshProUGUI>();
        uiManager.GetType()
            .GetField("winnerText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(uiManager, winnerText);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(uiGO);
    }

    [Test]
    public void UpdateActivePlayer_ChangesText()
    {
        var playerGO = new GameObject("PlayerGO");
        var player = playerGO.AddComponent<Player>();
        player.Initialize(1, "Alice", new Position { x = 0, y = 0 }, Color.red);

        uiManager.UpdateActivePlayer(player);

        Assert.AreEqual("Player to Move: Alice", activePlayerText.text);

        Object.DestroyImmediate(playerGO);
    }
}
