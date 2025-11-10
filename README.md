# Volthera: The Dominion - DEV

## Before running
- Download and install the latest version of Unity Hub: https://unity.com/download
- Open the project in Unity (recommended version: 2022.3 LTS)
- Open the `Main.unity` scene located in `Assets/Scenes/`
- Press `Play` to start the game in Editor

---

## Technologies used

### Development environment
| Tool / Technology       | Purpose                                                  |
|-------------------------|----------------------------------------------------------|
| Unity 2022.3 LTS        | Game engine for visuals, input handling, and UI updates |
| C#                      | Game logic and class-based architecture                  |
| Visual Studio Code      | Main code editor with Unity integration and debugging    |
| Git                     | Local version control                                    |
| GitHub                  | Remote repository and team collaboration                |
| Markdown                | Documentation, changelogs, and README files             |

### Graphics and UI
| Tool / Component        | Purpose                                                  |
|-------------------------|----------------------------------------------------------|
| Figma                   | Icon and species symbol design                           |
| Adobe Photoshop         | Graphic asset editing                                    |
| Unity Canvas System     | UI layout and interactive elements (buttons, texts, bars)|

### Save and load system
| Library / Component     | Purpose                                                  |
|-------------------------|----------------------------------------------------------|
| JsonUtility             | Basic JSON serialization/deserialization                 |
| Newtonsoft.Json (Json.NET)| Advanced object handling for save/load logic         |
| persistentDataPath      | Platform-independent save file storage (Windows/Linux)   |

### Audio
| Component               | Purpose                                                  |
|-------------------------|----------------------------------------------------------|
| AudioSource             | Playing sound effects and background music               |
| AudioClip               | Storing and referencing audio files                      |

## Project structure
```
volthera/
├── Assets/
│ ├── Scenes/
│ │ └── Main.unity
│ ├── Scripts/
│ │ └── Game.cs
├── README.md
├── .gitignore
```
