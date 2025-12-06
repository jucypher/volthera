using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GridManager : MonoBehaviour
{
    private int BoardSize;

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private float boardWorldSize = 4f; // Teljes pálya mérete Unity egységben

    //private int BoardSize;
    private Tile[,] Board;
    private Player[] players;

    public int PlayerCount = 2;

    private float margin = 0.05f;
    private Color[] PlayerColors = { Color.red, Color.blue, Color.green, Color.magenta };
    public Player ActivePlayer { get; private set; }

    public int WinScore { get; private set; }

    [SerializeField] private float WinScoreMultiplier = 0.7f;

    public int Round { get; private set; } = 1;


    void Start()
    {
        PlayerCount = MenuManager.PlayerCountSelected;
        BoardSize = MenuManager.BoardSizeSelected;

        Board = new Tile[BoardSize, BoardSize];

        if (PlayerPrefs.GetInt("ShouldLoadGame", 0) == 1)
        {
            PlayerPrefs.SetInt("ShouldLoadGame", 0);
            LoadSavedGame();
            return;
        }

        if (uiManager != null)
            uiManager.UpdateRound(Round);

        GenerateGrid();
        PlacePlayersInCorners();
        PlaceSpecialTilesForPlayers();
        PlaceBoostTiles();


        ActivePlayer = players[0];

        if (uiManager == null)
            Debug.LogError("GridManager: uiManager reference is NOT set!");

        StartCoroutine(DelayedUIUpdate());

        WinScore = Mathf.CeilToInt((BoardSize * BoardSize) / ((float)PlayerCount * WinScoreMultiplier));

        if (uiManager != null)
            uiManager.DisplayWinScore(WinScore);
        Debug.Log($"Game started. Win condition: {WinScore} points.");
    }


    private IEnumerator DelayedUIUpdate()
    {
        yield return null;
        if (uiManager != null)
        {
            uiManager.UpdateActivePlayer(ActivePlayer);
            uiManager.UpdateScores(players);
        }
    }

    void Update()
    {
        HandleMouseClick();
    }

    void HandleMouseClick()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Map"));

        if (hit.collider != null)
        {
            Tile clickedTile = hit.collider.GetComponent<Tile>();
            if (clickedTile != null && ActivePlayer != null)
            {
                MovePlayerTo(ActivePlayer, clickedTile.GridPos);
            }
        }
    }

    public void MovePlayerTo(Player player, Position targetPos)
    {
        if (targetPos.x < 0 || targetPos.x >= BoardSize || targetPos.y < 0 || targetPos.y >= BoardSize)
            return;

        int dx = Mathf.Abs(targetPos.x - player.CurrentPosition.x);
        int dy = Mathf.Abs(targetPos.y - player.CurrentPosition.y);

        if (!((dx == 1 && dy == 0) || (dx == 0 && dy == 1)))
        {
            Debug.Log($"{player.PlayerName} cannot move diagonally or more than one tile!");
            return;
        }

        Tile targetTile = Board[targetPos.x, targetPos.y];
        if (targetTile == null) return;

        // Ha boost tile, először adjuk a boostot
        if (targetTile.IsBoostTile)
        {
            targetTile.ApplyBoost(player);
            if (uiManager != null)
                uiManager.UpdateScores(players);
        }

        // Ezután kezeljük a saját tulajdonú vagy üres tile-okat
        HandleSpecialTileVisit(player, targetTile);

        if (targetTile.Owner == player)
        {
            player.CurrentPosition = targetPos;
            player.transform.position = targetTile.transform.position;

            Debug.Log($"{player.PlayerName} stepped on own tile. No points.");
            NextPlayerTurn(player);
            return;
        }

        if (!targetTile.IsOccupied())
        {
            targetTile.SetOwner(player);
            player.Score += 1;
            Debug.Log($"{player.PlayerName} captured empty tile. Score: {player.Score}");

            CheckWinCondition(player);

            player.CurrentPosition = targetPos;
            player.transform.position = targetTile.transform.position;

            if (uiManager != null)
                uiManager.UpdateScores(players);

            NextPlayerTurn(player);
            return;
        }


        Player defender = targetTile.Owner;

        float attackerChance = 0.5f * player.BattleMultiplier;
        float defenderChance = 0.5f * defender.BattleMultiplier;
        float total = attackerChance + defenderChance;
        float roll = Random.value;

        Player winner = (roll < attackerChance / total) ? player : defender;
        Player loser = (winner == player ? defender : player);

        targetTile.SetOwner(winner);

        winner.Score += 1;
        loser.Score = Mathf.Max(0, loser.Score - 1);

        if (uiManager != null)
            uiManager.UpdateScores(players);

        Debug.Log($"{player.PlayerName} attacked {defender.PlayerName}!");
        Debug.Log($"Winner: {winner.PlayerName} ({winner.Score})");
        Debug.Log($"Loser: {loser.PlayerName} ({loser.Score})");

        CheckWinCondition(winner);
        if (winner == player)
        {
            player.CurrentPosition = targetPos;
            player.transform.position = targetTile.transform.position; // <-- javítva
        }

        NextPlayerTurn(player);
    }


    private void CheckWinCondition(Player player)
    {
        if (player.Score >= WinScore)
        {
            Debug.Log($"{player.PlayerName} reached {WinScore} points and WINS the game!");

            ActivePlayer = null;

            if (uiManager != null)
                uiManager.SetWinner(player);
        }
    }


    private void NextPlayerTurn(Player current)
    {
        if (ActivePlayer == null) return;

        int index = System.Array.IndexOf(players, current);
        int nextIndex = (index + 1) % PlayerCount;
        ActivePlayer = players[nextIndex];

        // Ha visszaértünk az első játékoshoz, nő a kör
        if (nextIndex == 0)
        {
            Round++;
            if (uiManager != null)
                uiManager.UpdateRound(Round);
        }

        if (uiManager != null)
            uiManager.UpdateActivePlayer(ActivePlayer);
    }



    void GenerateGrid()
    {
        float tileSize = (boardWorldSize / BoardSize) - margin;

        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                Vector2 pos = new Vector2(x * (tileSize + margin), y * (tileSize + margin));
                GameObject t = Instantiate(_tilePrefab, pos, Quaternion.identity, transform);
                t.transform.localScale = new Vector3(tileSize, tileSize, 1);

                Tile tile = t.GetComponent<Tile>();
                tile.Initialize(x, y);
                Board[x, y] = tile;
            }
        }
    }

    void PlaceSpecialTilesForPlayers()
    {
        System.Random random = new System.Random();
        HashSet<Position> occupied = new HashSet<Position>();

        for (int i = 0; i < PlayerCount; i++)
        {
            bool placed = false;
            Position pos;

            while (!placed)
            {
                int x = random.Next(BoardSize);
                int y = random.Next(BoardSize);
                pos = new Position(x, y);


                if ((x == 0 && y == 0) ||
                    (x == 0 && y == BoardSize - 1) ||
                    (x == BoardSize - 1 && y == 0) ||
                    (x == BoardSize - 1 && y == BoardSize - 1))
                    continue;

                if (!occupied.Contains(pos))
                {

                    Board[x, y].SetSpecial(PlayerColors[i], players[i]);
                    occupied.Add(pos);
                    placed = true;
                }
            }
        }
    }


    void PlacePlayersInCorners()
    {
        players = new Player[PlayerCount];

        Position[] corners = PlayerCount switch
        {
            2 => new Position[] { new Position(0, 0), new Position(BoardSize - 1, BoardSize - 1) },
            3 => new Position[] { new Position(0, 0), new Position(0, BoardSize - 1), new Position(BoardSize - 1, BoardSize - 1) },
            _ => new Position[] { new Position(0, 0), new Position(0, BoardSize - 1), new Position(BoardSize - 1, 0), new Position(BoardSize - 1, BoardSize - 1) }
        };

        float tileSize = (boardWorldSize / BoardSize) - margin;

        for (int i = 0; i < PlayerCount; i++)
        {
            Tile startTile = Board[corners[i].x, corners[i].y];
            Vector2 playerWorldPos = startTile.transform.position; // <--- itt hozzuk létre

            GameObject pGO = Instantiate(_playerPrefab, playerWorldPos, Quaternion.identity, transform);
            float playerScale = tileSize * 0.6f;
            pGO.transform.localScale = new Vector3(playerScale, playerScale, 1);

            Player player = pGO.GetComponent<Player>();
            player.Initialize(i + 1, "Player " + (i + 1), corners[i], PlayerColors[i], playerWorldPos); // <-- használjuk a világpozíciót
            players[i] = player;

            startTile.SetOwner(player);
            startTile.HighlightUnderPlayer(PlayerColors[i]);
        }
    }


    private void HandleSpecialTileVisit(Player visitor, Tile tile)
    {
        // csak special tile esetén érdekes
        if (!tile.IsSpecialTile) return;

        Player owner = tile.SpecialOwner;
        if (owner == null) return;

        // ---------- SAJÁT SPECIAL TILE ----------
        if (owner == visitor)
        {
            // csak akkor adjon buffot, ha még nincs
            if (!visitor.HasBuff)
            {
                visitor.BattleMultiplier += 0.5f;   // vagy amennyi kell
                visitor.HasBuff = true;
                Debug.Log($"{visitor.PlayerName} gained boost from their own special tile!");
            }
            return;
        }

        // ---------- MÁS SPECIAL TILE (CSAPDA) ----------
        // ide akkor jutunk, ha owner != visitor
        if (visitor.HasBuff || visitor.BattleMultiplier > 1f)
        {
            visitor.HasBuff = false;
            visitor.BattleMultiplier = 1.0f;
            Debug.Log($"{visitor.PlayerName} stepped on {owner.PlayerName}'s special tile and LOST all boosts!");
        }
    }



    private Tile FindTileBySpecialOwner(Player player)
    {
        if (player == null) return null;
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                var t = Board[x, y];
                if (t != null && t.IsSpecialTile && t.SpecialOwner == player)
                    return t;
            }
        }
        return null;
    }

    public void SaveGame(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            Debug.LogError("Save failed: empty filename!");
            return;
        }

        if (ActivePlayer == null)
        {
            Debug.LogWarning("Game already finished, saving disabled.");
            return;
        }

        GameSave save = new GameSave();

        // ✅ alap játékadatok
        save.boardSize = BoardSize;
        save.round = Round;
        save.winScore = WinScore;

        // ✅ játékosok mentése
        save.players = new GameSave.PlayerData[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            Player p = players[i];
            GameSave.PlayerData pd = new GameSave.PlayerData
            {
                playerName = p.PlayerName,
                id = p.ID,
                posX = p.CurrentPosition.x,
                posY = p.CurrentPosition.y,
                score = p.Score,
                battleMultiplier = p.BattleMultiplier,
                hasBuff = p.HasBuff,
                canReceiveSpecialBuff = p.CanReceiveSpecialBuff,
                colorR = p.Color.r,
                colorG = p.Color.g,
                colorB = p.Color.b
            };
            save.players[i] = pd;
        }

        // ✅ tile-ok mentése (+ BOOST!)
        save.tiles = new GameSave.TileData[BoardSize * BoardSize];
        int index = 0;

        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                Tile t = Board[x, y];

                GameSave.TileData td = new GameSave.TileData
                {
                    x = t.GridPos.x,
                    y = t.GridPos.y,

                    ownerName = t.Owner != null ? t.Owner.PlayerName : null,
                    specialOwnerName = t.SpecialOwner != null ? t.SpecialOwner.PlayerName : null,
                    specialCapturedBy = t.SpecialCapturedBy != null ? t.SpecialCapturedBy.PlayerName : null,

                    colorR = t.BaseColor.r,
                    colorG = t.BaseColor.g,
                    colorB = t.BaseColor.b,

                    // ✅ BOOST TILE MENTÉS
                    isBoostTile = t.IsBoostTile,
                    boostUsed = t.BoostUsed
                };

                save.tiles[index++] = td;
            }
        }

        save.activePlayerName = ActivePlayer != null ? ActivePlayer.PlayerName : null;

        string json = JsonUtility.ToJson(save, true);

        string folder = Path.Combine(Application.dataPath, "Saves");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = Path.Combine(folder, fileName + ".json");

        if (File.Exists(path))
            Debug.LogWarning("Save already exists and will be overwritten!");

        File.WriteAllText(path, json);

        Debug.Log($"✅ Game saved to: {path}");
    }


    private void LoadSavedGame()
    {
        string path = Path.Combine(Application.dataPath, "Saves/savegame.json");
        if (!File.Exists(path))
        {
            Debug.LogError("SAVE FILE NOT FOUND! Starting new game instead.");
            GenerateGrid();
            PlacePlayersInCorners();
            PlaceSpecialTilesForPlayers();
            ActivePlayer = players[0];
            StartCoroutine(DelayedUIUpdate());
            return;
        }

        string json = File.ReadAllText(path);
        GameSave save = JsonUtility.FromJson<GameSave>(json);

        GenerateGrid();

        players = new Player[save.players.Length];
        PlayerCount = save.players.Length;

        Dictionary<string, Player> playerMap = new();

        for (int i = 0; i < save.players.Length; i++)
        {
            var p = save.players[i];

            Vector2 worldPos = Board[p.posX, p.posY].transform.position;

            GameObject pGO = Instantiate(
                _playerPrefab,
                worldPos,
                Quaternion.identity,
                transform
            );

            Player player = pGO.GetComponent<Player>();
            Color color = new Color(p.colorR, p.colorG, p.colorB);

            player.Initialize(
                p.id,
                p.playerName,
                new Position(p.posX, p.posY),
                color,
                worldPos
            );

            player.Score = p.score;
            player.BattleMultiplier = p.battleMultiplier;
            player.HasBuff = p.hasBuff;
            player.CanReceiveSpecialBuff = p.canReceiveSpecialBuff;

            players[i] = player;
            playerMap[p.playerName] = player;

            Board[p.posX, p.posY].HighlightUnderPlayer(color);
            Board[p.posX, p.posY].SetOwner(player);
        }


        foreach (var t in save.tiles)
        {
            Tile tile = Board[t.x, t.y];

            tile.BaseColor = new Color(t.colorR, t.colorG, t.colorB);

            Player specialOwner = null;
            Player capturedBy = null;

            if (!string.IsNullOrEmpty(t.specialOwnerName) && playerMap.ContainsKey(t.specialOwnerName))
                specialOwner = playerMap[t.specialOwnerName];

            if (!string.IsNullOrEmpty(t.specialCapturedBy) && playerMap.ContainsKey(t.specialCapturedBy))
                capturedBy = playerMap[t.specialCapturedBy];

            tile.RestoreSpecial(specialOwner, capturedBy);

            if (!string.IsNullOrEmpty(t.ownerName) && playerMap.ContainsKey(t.ownerName))
                tile.SetOwner(playerMap[t.ownerName]);
        }



        if (!string.IsNullOrEmpty(save.activePlayerName) && playerMap.ContainsKey(save.activePlayerName))
            ActivePlayer = playerMap[save.activePlayerName];
        else
            ActivePlayer = players[0];

        Debug.Log($"Loaded game. Active player: {ActivePlayer.PlayerName}");

        StartCoroutine(DelayedUIUpdate());
    }

    void PlaceBoostTiles()
    {
        int boostCount = MenuManager.BoostTileCount;
        System.Random random = new System.Random();
        HashSet<Position> occupied = new HashSet<Position>();

        // Ne tegyük oda, ahol a special tile-ok vannak
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (Board[x, y].IsSpecialTile)
                    occupied.Add(Board[x, y].GridPos);
            }
        }

        // Ne tegyük a játékosok kezdőpozíciójára
        foreach (var p in players)
            occupied.Add(p.CurrentPosition);

        int placed = 0;
        while (placed < boostCount)
        {
            int x = random.Next(BoardSize);
            int y = random.Next(BoardSize);
            Position pos = new Position(x, y);

            if (occupied.Contains(pos))
                continue;

            Board[x, y].SetBoostTile();
            occupied.Add(pos);
            placed++;
        }
    }


}
