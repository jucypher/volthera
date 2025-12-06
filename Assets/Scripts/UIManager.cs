using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text activePlayerText;
    [SerializeField] private TMP_Text[] playerScoreTexts;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TMP_Text winScoreText;
    [SerializeField] private TMP_Text roundText;

    // ✅ ÚJ: SAVE POPUP ELEMEK
    [SerializeField] private GameObject savePopup;
    [SerializeField] private TMP_InputField saveNameInput;

    public void DisplayWinScore(int score)
    {
        if (winScoreText != null)
            winScoreText.text = $"Win at: {score} points";
    }

    public void UpdateRound(int round)
    {
        if (roundText != null)
            roundText.text = $"Round: {round}";
    }

    public void UpdateActivePlayer(Player currentPlayer)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("UIManager.UpdateActivePlayer: currentPlayer is NULL");
            return;
        }

        if (activePlayerText != null)
            activePlayerText.text = $"Player to Move: {currentPlayer.PlayerName}";
        else
            Debug.LogError("UIManager.UpdateActivePlayer: activePlayerText is NULL!");
    }

    public void UpdateScores(Player[] players)
    {
        if (players == null || playerScoreTexts == null)
            return;

        int playerCount = players.Length;

        for (int i = 0; i < playerScoreTexts.Length; i++)
        {
            if (i < playerCount)
            {
                playerScoreTexts[i].gameObject.SetActive(true);
                float boost = players[i].BattleMultiplier;
                string boostText = boost > 1f ? $" (Boost: {boost:0.0}×)" : "";
                playerScoreTexts[i].text = $"{players[i].PlayerName}: {players[i].Score}{boostText}";
            }
            else
            {
                playerScoreTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetWinner(Player winner)
    {
        if (winnerText != null)
        {
            winnerText.gameObject.SetActive(true);
            winnerText.text = $"{winner.PlayerName} WINS!";
        }

        Debug.Log($"[UIManager] Winner displayed: {winner.PlayerName}");
    }

    // =========================================================
    // ================= SAVE POPUP LOGIKA =====================
    // =========================================================

    // ✅ EZ CSAK A POPUPOT NYITJA MEG (fő SAVE gomb)
    public void OnSaveButtonClicked()
    {
        if (savePopup != null)
        {
            savePopup.SetActive(true);
            saveNameInput.text = "";
        }
        else
        {
            Debug.LogError("Save popup is not assigned!");
        }
    }

    // ✅ EZ A POPUP SAVE GOMB – TÉNYLEGES MENTÉS
    public void OnConfirmSaveClicked()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager is not assigned!");
            return;
        }

        string fileName = saveNameInput.text;

        if (string.IsNullOrWhiteSpace(fileName))
        {
            Debug.LogWarning("Nem adtál meg mentési nevet!");
            return;
        }

        gridManager.SaveGame(fileName);
        savePopup.SetActive(false);
    }

    // ✅ POPUP CANCEL
    public void OnCancelSaveClicked()
    {
        if (savePopup != null)
            savePopup.SetActive(false);
    }
}
