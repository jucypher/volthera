using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("Game started");
        LoadGame(); // próbál mentést betölteni
        SetState(GameState.Playing); // ha nincs mentés, új játék
    }

    void Update()
    {
        // Globális input figyelés (pl. pause)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Game state changed to: " + newState);

        switch (newState)
        {
            case GameState.MainMenu:
                // UI megjelenítése
                break;
            case GameState.Playing:
                // Pálya betöltése, egységek aktiválása
                break;
            case GameState.Paused:
                // Játék megállítása
                break;
            case GameState.GameOver:
                // Vége képernyő, mentés törlése
                break;
        }
    }

    void TogglePause()
    {
        if (CurrentState == GameState.Playing)
            SetState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            SetState(GameState.Playing);
    }

    void LoadGame()
    {
        // Itt majd JSON-ből töltünk mentést
        Debug.Log("Attempting to load saved game...");
        // Ha nincs mentés, új játék indul
    }

    public void SaveGame()
    {
        // Itt majd JSON-be mentjük az állást
        Debug.Log("Saving game...");
    }
}
