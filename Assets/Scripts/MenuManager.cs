using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown playerCountDropdown;
    [SerializeField] private TMP_Dropdown boardSizeDropdown;

    [SerializeField] private TMP_InputField boostCountInput;

    public static int BoostTileCount = 2;

    public static int BoardSizeSelected = 4;
    public static int PlayerCountSelected = 2;

    public static string SelectedSaveFile = null;


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

        if (boostCountInput != null)
        {
            boostCountInput.text = BoostTileCount.ToString();
            boostCountInput.onEndEdit.AddListener(OnBoostCountChanged);
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

    private void OnBoostCountChanged(string value)
    {
        if (int.TryParse(value, out int count))
        {
            BoostTileCount = Mathf.Max(0, count); // ne legyen negatív
            Debug.Log($"Boost tiles selected: {BoostTileCount}");
        }
    }
}
