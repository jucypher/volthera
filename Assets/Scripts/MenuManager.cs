using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown playerCountDropdown;

    private int selectedPlayerCount = 2;

    private void Start()
    {
        if (playerCountDropdown != null)
        {
            playerCountDropdown.ClearOptions();
            playerCountDropdown.AddOptions(new System.Collections.Generic.List<string> { "2", "3", "4" });
            playerCountDropdown.value = 0;
            playerCountDropdown.onValueChanged.AddListener(OnPlayerCountChanged);
        }
    }

    public static int PlayerCountSelected = 2;

    private void OnPlayerCountChanged(int value)
    {
        PlayerCountSelected = value + 2;
        Debug.Log($"Selected player count: {PlayerCountSelected}");
    }

    public void OnPlayClicked()
    {
        Debug.Log("Play clicked");
        SceneManager.LoadScene("Main");
    }


    public void OnSaveClicked()
    {
        Debug.Log("Save clicked (currently does nothing)");
    }

    public void OnSaveAndExitClicked()
    {
        Debug.Log("Save & Exit clicked (currently does nothing)");
    }

    public void OnContinueClicked()
    {
        Debug.Log("Continue clicked");

        // Betöltjük a játék Scene-t anélkül, hogy resetelnénk
        SceneManager.LoadScene("Main"); // ugyanaz a Scene, mint a Grid
    }

    public void OnExitClicked()
    {
        Debug.Log("Exit clicked");
        Application.Quit();
    }
}
