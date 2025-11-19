using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text activePlayerText;
    [SerializeField] private TMP_Text[] playerScoreTexts;

    public void UpdateActivePlayer(Player currentPlayer)
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("UIManager.UpdateActivePlayer: currentPlayer is NULL");
            return;
        }

        if (activePlayerText != null)
        {
            activePlayerText.text = $"Player to Move: {currentPlayer.PlayerName}";
        }
        else
        {
            Debug.LogError("UIManager.UpdateActivePlayer: activePlayerText is NULL!");
        }
    }

    public void UpdateScores(Player[] players)
    {
        if (players == null)
        {
            Debug.LogError("UIManager.UpdateScores: players array is NULL!");
            return;
        }

        if (playerScoreTexts == null)
        {
            Debug.LogError("UIManager.UpdateScores: playerScoreTexts array is NULL!");
            return;
        }

        int len = Mathf.Min(players.Length, playerScoreTexts.Length);

        for (int i = 0; i < len; i++)
        {
            var p = players[i];
            var txt = playerScoreTexts[i];

            if (txt != null)
            {
                txt.text = $"{p.PlayerName}: {p.Score}";
            }
            else
            {
                Debug.LogError($"UIManager.UpdateScores: playerScoreTexts[{i}] is NULL!");
            }
        }
    }
}
