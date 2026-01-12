# Volthera: The Dominion - DEV

## Before running

- Download and install the latest version of Unity Hub: https://unity.com/download
- Open the project in Unity (recommended version: 2022.3 LTS)
- Open the `MainMenu.unity` scene located in `Assets/Scenes/`
- Press `Play` to start the game in Editor

---

## Building and Running Instructions

### Build the Project (Optional)

1. In Unity Editor, go to **File > Build Settings**
2. Select your target platform (Windows, Mac, Linux)
3. Ensure `MainMenu.unity` is listed in the **Scenes in Build**
4. Click **Build** and choose a destination folder
5. Unity will create an executable (`.exe` / `.app` / `.x86_64`) in the selected folder

### Running the Built Game

- Locate the executable file in your build folder
- Run the executable to start the game outside of Unity Editor
- All game save files are stored in `Assets/Saves/` (or relative to the build folder)

---

## Technologies used

### Development environment

| Tool / Technology  | Purpose                                                 |
| ------------------ | ------------------------------------------------------- |
| Unity 2022.3 LTS   | Game engine for visuals, input handling, and UI updates |
| C#                 | Game logic and class-based architecture                 |
| Visual Studio Code | Main code editor with Unity integration and debugging   |
| Git                | Local version control                                   |
| GitHub             | Remote repository and team collaboration                |

### Graphics and UI

| Tool / Component    | Purpose                                                   |
| ------------------- | --------------------------------------------------------- |
| Adobe Photoshop     | Graphic asset editing                                     |
| Unity Canvas System | UI layout and interactive elements (buttons, texts, bars) |

### Save and load system

| Library / Component | Purpose                                     |
| ------------------- | ------------------------------------------- |
| SaveLoadListUI.cs   | Dynamically populate save/load menu buttons |
| GridManager.cs      | SaveGame() and LoadGame() methods           |
| Saves Folder        | Stores saved games as JSON files            |

---

## Project structure

```
volthera/
├── Assets/
│ ├── Scenes/
│ │ ├── MainMenu.unity
│ │ └── Main.unity
│ ├── Prefabs/
│ ├── Saves/
│ ├── Sprites/
│ └── Scripts/
│ │ ├── GameSave.cs
│ │ └── GridManager.cs
│ │ └── MenuManager.cs
│ │ └── Player.cs
│ │ └── Positions.cs
│ │ └── SaveLoadListUI.cs
│ │ └── Tile.cs
│ │ └── UIManager.cs
│ ├── Tests/
├── README.md
├── .gitignore
```

---

## Gameplay and Mechanics

### Board and Units

- The game uses a configurable **grid board** (default 4x4, options: 3x3–6x6).
- Players move their **units** one tile at a time (up, down, left, right).
- Capturing an empty tile gives **+1 energy**.
- Moving onto a tile occupied by another player triggers a **mini-battle**:
  - Winner gains +2 points
  - Loser loses -1 point

### Turn and Round Management

- A **turn** ends after a player moves.
- A **round** ends when all players have taken their turn.
- Current round number and scores are displayed continuously.

### Default Settings

- Player count: 2
- Board size: 4x4
- Boost tiles: 2

### Boost and Special Fields

- Each player has a unique special tile randomly placed at the start of the game.
- Reaching the special tile increases battle success probability.
- Boost tiles are optional, configurable at game start, and can be claimed only once.

---

## Save / Load System

- Game state can be saved in **JSON format** at any turn.
- Multiple saves are supported.
- Players can select which save to load from a dynamically generated menu.
- Save and load are handled via `GridManager.cs` and `SaveLoadListUI.cs`.

---

## Debugging & Feedback

- Game logs indicate:

  - Current player and turn
  - Score changes
  - Save/load events
  - Invalid moves

---

## Use Cases

| ID  | Use Case                    | Description                                                                | Priority |
| --- | --------------------------- | -------------------------------------------------------------------------- | -------- |
| 1   | Move Unit (player)          | Move a unit to an adjacent tile.                                           | High     |
| 2   | Capture Tile (player)       | Claim an empty tile and gain +1 energy.                                    | High     |
| 3   | Engage Battle (player)      | Battle occurs when moving onto an occupied tile; winner +2, loser -1.      | High     |
| 4   | End Turn (player)           | Ends the current player’s turn; next player moves.                         | High     |
| 5   | End Round (game)            | Round ends after all players have moved.                                   | High     |
| 6   | Display Scores (game)       | Player scores are updated and displayed after each turn.                   | Medium   |
| 7   | Save Game (game)            | Save current game state to JSON.                                           | Medium   |
| 8   | Load Game (game)            | Load a previously saved game state.                                        | Medium   |
| 9   | Visual Feedback (game)      | Show tile ownership and unit positions visually.                           | Medium   |
| 10  | Invalid Move Warning (game) | Warn the player on invalid moves; turn is not consumed.                    | Low      |
| 11  | Reach Special Tile (player) | Player reaches faction-specific or booster tile to improve battle chances. | Medium   |

---
