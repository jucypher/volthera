using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private const int BoardSize = 4;

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private UIManager uiManager;   // <<< IDE KAPJA A UI MANAGERT

    public int PlayerCount = 2;

    private Tile[,] Board = new Tile[BoardSize, BoardSize];
    private Player[] players;

    private Color[] PlayerColors = { Color.red, Color.blue, Color.green, Color.magenta };
    public Player ActivePlayer { get; private set; }

    void Start()
    {
        GenerateGrid();
        PlaceSpecialTilesForPlayers();
        PlacePlayersInCorners();

        ActivePlayer = players[0];

        if (uiManager == null)
            Debug.LogError("GridManager: uiManager reference is NOT set in Inspector!");

        StartCoroutine(DelayedUIUpdate());
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

        // Csak 1 tile mozgás
        if (!((dx == 1 && dy == 0) || (dx == 0 && dy == 1)))
        {
            Debug.Log($"{player.PlayerName} cannot move diagonally or more than one tile!");
            return;
        }

        Tile targetTile = Board[targetPos.x, targetPos.y];
        if (targetTile == null) return;

        // ===== 1. SAJÁT MEZŐ =====
        if (targetTile.Owner == player)
        {
            player.CurrentPosition = targetPos;
            player.transform.position = new Vector2(targetPos.x, targetPos.y);

            Debug.Log($"{player.PlayerName} stepped on own tile. No points.");

            NextPlayerTurn(player);
            return;
        }

        // ===== 2. ÜRES MEZŐ =====
        if (!targetTile.IsOccupied())
        {
            targetTile.SetOwner(player);
            player.Score += 1;

            Debug.Log($"{player.PlayerName} captured empty tile. Score: {player.Score}");

            player.CurrentPosition = targetPos;
            player.transform.position = new Vector2(targetPos.x, targetPos.y);

            if (uiManager != null)
                uiManager.UpdateScores(players);

            NextPlayerTurn(player);
            return;
        }

        // ===== 3. CSATA =====
        Player defender = targetTile.Owner;
        Player winner = Random.value < 0.5f ? player : defender;
        Player loser = (winner == player ? defender : player);

        targetTile.SetOwner(winner);

        winner.Score += 1;
        loser.Score = Mathf.Max(0, loser.Score - 1);

        if (uiManager != null)
            uiManager.UpdateScores(players);

        Debug.Log($"{player.PlayerName} attacked {defender.PlayerName}!");
        Debug.Log($"Winner: {winner.PlayerName} ({winner.Score})");
        Debug.Log($"Loser: {loser.PlayerName} ({loser.Score})");

        // Nyertes lép a tile-ra
        if (winner == player)
        {
            player.CurrentPosition = targetPos;
            player.transform.position = new Vector2(targetPos.x, targetPos.y);
        }
        // Vesztes nem mozdul

        NextPlayerTurn(player);
    }

    // ===== NEXT PLAYER =====
    private void NextPlayerTurn(Player current)
    {
        int index = System.Array.IndexOf(players, current);
        int nextIndex = (index + 1) % PlayerCount;
        ActivePlayer = players[nextIndex];

        if (uiManager != null)
            uiManager.UpdateActivePlayer(ActivePlayer);
    }

    // ===== GRID CREATION =====
    void GenerateGrid()
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                GameObject t = Instantiate(_tilePrefab, new Vector2(x, y), Quaternion.identity);
                t.transform.parent = transform;

                Tile tile = t.GetComponent<Tile>();
                tile.Initialize(x, y);
                Board[x, y] = tile;
            }
        }
    }

    // ===== SPECIAL TILES =====
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
                    Board[x, y].SetSpecial(PlayerColors[i]);
                    occupied.Add(pos);
                    placed = true;
                }
            }
        }
    }

    // ===== PLAYER SPAWN =====
    void PlacePlayersInCorners()
    {
        players = new Player[PlayerCount];

        Position[] corners = PlayerCount switch
        {
            2 => new Position[] { new Position(0, 0), new Position(BoardSize - 1, BoardSize - 1) },
            3 => new Position[] { new Position(0, 0), new Position(0, BoardSize - 1), new Position(BoardSize - 1, BoardSize - 1) },
            _ => new Position[] { new Position(0, 0), new Position(0, BoardSize - 1), new Position(BoardSize - 1, 0), new Position(BoardSize - 1, BoardSize - 1) }
        };

        for (int i = 0; i < PlayerCount; i++)
        {
            GameObject pGO = Instantiate(_playerPrefab, new Vector2(corners[i].x, corners[i].y), Quaternion.identity);
            pGO.transform.parent = transform;

            Player player = pGO.GetComponent<Player>();
            player.Initialize(i + 1, "Player " + (i + 1), corners[i], PlayerColors[i]);
            players[i] = player;

            Tile startTile = Board[corners[i].x, corners[i].y];
            if (startTile != null)
            {
                startTile.SetOwner(player); // saját mező, nincs pont érte
                startTile.HighlightUnderPlayer(PlayerColors[i]);
            }
        }
    }
}
