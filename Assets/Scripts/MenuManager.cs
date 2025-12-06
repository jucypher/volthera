using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown playerCountDropdown;
    [SerializeField] private TMP_Dropdown boardSizeDropdown; // Hozzáadva

    public static int BoardSizeSelected = 4;
    public static int PlayerCountSelected = 2;

    private void Start()
    {
        // Player count dropdown
        if (playerCountDropdown != null)
        {
            playerCountDropdown.ClearOptions();
            playerCountDropdown.AddOptions(new System.Collections.Generic.List<string> { "2", "3", "4" });
            playerCountDropdown.value = 0;
            playerCountDropdown.onValueChanged.AddListener(OnPlayerCountChanged);
        }

        // Board size dropdown
        if (boardSizeDropdown != null)
        {
            boardSizeDropdown.ClearOptions();
            boardSizeDropdown.AddOptions(new System.Collections.Generic.List<string> { "3", "4", "5", "6" });
            boardSizeDropdown.value = 1; // alapértelmezett 4x4
            boardSizeDropdown.onValueChanged.AddListener(OnBoardSizeChanged);
        }
    }

    private void OnPlayerCountChanged(int value)
    {
        PlayerCountSelected = value + 2;
        Debug.Log($"Selected player count: {PlayerCountSelected}");
    }

    private void OnBoardSizeChanged(int value)
    {
        BoardSizeSelected = value + 3;
        Debug.Log($"Selected board size: {BoardSizeSelected}");
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnContinueClicked()
    {
        PlayerPrefs.SetInt("ShouldLoadGame", 1);
        SceneManager.LoadScene("Main");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
