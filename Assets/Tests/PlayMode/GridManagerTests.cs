using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Reflection;

public class GridManagerTests
{
    private GameObject gridGO;
    private GridManager grid;

    private GameObject tilePrefab;
    private GameObject playerPrefab;

    [SetUp]
    public void SetUp()
    {
        MenuManager.PlayerCountSelected = 2;
        MenuManager.BoardSizeSelected = 3;
        MenuManager.BoostTileCount = 0;
        MenuManager.SelectedSaveFile = null;
        PlayerPrefs.SetInt("ShouldLoadGame", 0);

        gridGO = new GameObject("GridManagerTest");
        grid = gridGO.AddComponent<GridManager>();

        tilePrefab = new GameObject("TilePrefab");
        tilePrefab.AddComponent<SpriteRenderer>();
        tilePrefab.AddComponent<Tile>();

        playerPrefab = new GameObject("PlayerPrefab");
        playerPrefab.AddComponent<SpriteRenderer>();
        playerPrefab.AddComponent<Player>();

        SetPrivateField(grid, "_tilePrefab", tilePrefab);
        SetPrivateField(grid, "_playerPrefab", playerPrefab);

        SetPrivateField(grid, "BoardSize", 3);
        grid.PlayerCount = 2;

        var boardArray = new Tile[3, 3];
        SetPrivateField(grid, "Board", boardArray);

        CallPrivateMethod(grid, "GenerateGrid");
        CallPrivateMethod(grid, "PlacePlayersInCorners");

        var players = (Player[])GetPrivateField(grid, "players");
        typeof(GridManager)
            .GetProperty("ActivePlayer")
            .SetValue(grid, players[0]);

        typeof(GridManager)
            .GetProperty("WinScore")
            .SetValue(grid, 5);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gridGO);
        Object.DestroyImmediate(tilePrefab);
        Object.DestroyImmediate(playerPrefab);
    }


    private void SetPrivateField(object target, string fieldName, object value)
    {
        target.GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(target, value);
    }

    private object GetPrivateField(object target, string fieldName)
    {
        return target.GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(target);
    }

    private void CallPrivateMethod(object target, string methodName, params object[] args)
    {
        target.GetType()
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance)
            .Invoke(target, args);
    }

    [Test]
    public void Grid_CreatesBoardWithCorrectSize()
    {
        var board = (Tile[,])GetPrivateField(grid, "Board");

        Assert.IsNotNull(board, "Board tömb null");
        Assert.AreEqual(3, board.GetLength(0), "Board X méret nem 3");
        Assert.AreEqual(3, board.GetLength(1), "Board Y méret nem 3");

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Assert.IsNotNull(board[x, y], $"Board[{x},{y}] null");
            }
        }
    }

    [Test]
    public void SaveGame_CreatesJsonFile()
    {
        string folder = Path.Combine(Application.dataPath, "Saves");
        string path = Path.Combine(folder, "testsave.json");

        if (File.Exists(path))
            File.Delete(path);

        var players = (Player[])GetPrivateField(grid, "players");
        typeof(GridManager)
            .GetProperty("ActivePlayer")
            .SetValue(grid, players[0]);

        grid.SaveGame("testsave");

        Assert.IsTrue(File.Exists(path), "SaveGame után nem jött létre a testsave.json!");
    }

    [Test]
    public void CheckWinCondition_DeletesCurrentSaveFile()
    {
        string folder = Path.Combine(Application.dataPath, "Saves");
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string fileName = "wintest";
        string path = Path.Combine(folder, fileName + ".json");
        File.WriteAllText(path, "DUMMY");

        MenuManager.SelectedSaveFile = fileName;

        var players = (Player[])GetPrivateField(grid, "players");

        typeof(GridManager)
            .GetProperty("WinScore")
            .SetValue(grid, 3);

        players[0].Score = 3;

        CallPrivateMethod(grid, "CheckWinCondition", players[0]);

        Assert.IsFalse(File.Exists(path), "Nyertes játék után a mentés NINCS törölve!");

        Assert.IsNull(MenuManager.SelectedSaveFile, "SelectedSaveFile nincs null-ra rakva a törlés után.");
    }

    [Test]
    public void MovePlayerTo_DoesNotAllowDiagonalMove()
    {
        var players = (Player[])GetPrivateField(grid, "players");
        var player = players[0];

        Position originalPos = player.CurrentPosition;
        Position diagonalTarget = new Position(originalPos.x + 1, originalPos.y + 1);

        grid.MovePlayerTo(player, diagonalTarget);

        Assert.AreEqual(originalPos.x, player.CurrentPosition.x);
        Assert.AreEqual(originalPos.y, player.CurrentPosition.y);
    }


    [Test]
    public void MovePlayerTo_EmptyTile_IncreasesScoreAndChangesOwner()
    {
        var board = (Tile[,])GetPrivateField(grid, "Board");
        var players = (Player[])GetPrivateField(grid, "players");
        var player = players[0];

        Position startPos = player.CurrentPosition;
        Position targetPos = new Position(startPos.x + 1, startPos.y);

        Tile targetTile = board[targetPos.x, targetPos.y];

        int oldScore = player.Score;

        grid.MovePlayerTo(player, targetPos);

        Assert.AreEqual(player, targetTile.Owner, "Target tile owner nem a játékos lett.");
        Assert.AreEqual(oldScore + 1, player.Score, "Score nem nőtt 1-gyel.");
        Assert.AreEqual(targetPos.x, player.CurrentPosition.x);
        Assert.AreEqual(targetPos.y, player.CurrentPosition.y);
    }
}
