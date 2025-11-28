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
        if (players == null || playerScoreTexts == null)
            return;

        int playerCount = players.Length;


        for (int i = 0; i < playerScoreTexts.Length; i++)
        {
            if (i < playerCount)
            {

                playerScoreTexts[i].gameObject.SetActive(true);
                playerScoreTexts[i].text = $"{players[i].PlayerName}: {players[i].Score}";
            }
            else
            {

                playerScoreTexts[i].gameObject.SetActive(false);
            }
        }
    }

}
