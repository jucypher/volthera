using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveLoadListUI : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject saveButtonPrefab;

    private string saveFolderPath;

    void Awake()
    {
        saveFolderPath = Path.Combine(Application.dataPath, "Saves");
        Debug.Log("Save mappa útvonala: " + saveFolderPath);
    }

    public void RefreshList()
    {
        Debug.Log("✅ RefreshList meghívva!");

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (!Directory.Exists(saveFolderPath))
        {
            Debug.LogWarning("❌ NINCS Assets/Saves MAPPA!");
            return;
        }

        string[] files = Directory.GetFiles(saveFolderPath, "*.json");
        Debug.Log("✅ Talált mentések száma: " + files.Length);

        foreach (string filePath in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            GameObject buttonGO = Instantiate(saveButtonPrefab, contentParent);
            TMP_Text txt = buttonGO.GetComponentInChildren<TMP_Text>();
            txt.text = fileName;

            buttonGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                MenuManager.SelectedSaveFile = fileName;
                PlayerPrefs.SetInt("ShouldLoadGame", 1);

                SceneManager.LoadScene("Main");
            });
        }
    }
}
