using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private const int BoardSize = 4;

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _playerPrefab;

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

    void PlaceSpecialTilesForPlayers()
    {
        System.Random random = new System.Random();
        HashSet<Position> occupied = new HashSet<Position>();

        for (int i = 0; i < PlayerCount; i++)
        {
            Position pos;
            bool placed = false;

            while (!placed)
            {
                int x = random.Next(BoardSize);
                int y = random.Next(BoardSize);
                pos = new Position(x, y);

                if ((x == 0 && y == 0) || (x == 0 && y == BoardSize - 1) ||
                    (x == BoardSize - 1 && y == 0) || (x == BoardSize - 1 && y == BoardSize - 1))
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

            Player playerScript = pGO.GetComponent<Player>();
            playerScript.Initialize(i + 1, "Player " + (i + 1), corners[i], PlayerColors[i]);
            players[i] = playerScript;

            // Highlight a kezdő pozíciót rögtön
            Tile startTile = Board[corners[i].x, corners[i].y];
            if (startTile != null)
                startTile.HighlightUnderPlayer(PlayerColors[i]);
        }
    }

    public void MovePlayerTo(Player player, Position targetPos)
    {
        // Határok ellenőrzése
        if (targetPos.x < 0 || targetPos.x >= BoardSize || targetPos.y < 0 || targetPos.y >= BoardSize)
            return;

        // Csak egységnyi lépés megengedett (vízszintes vagy függőleges)
        int dx = Mathf.Abs(targetPos.x - player.CurrentPosition.x);
        int dy = Mathf.Abs(targetPos.y - player.CurrentPosition.y);

        if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1))
        {
            Tile targetTile = Board[targetPos.x, targetPos.y];

            if (targetTile != null)
            {
                // Resetelődik minden előző “player szín”
                targetTile.SetPlayerColor(player.Color);

                // Mozgatás
                player.CurrentPosition = targetPos;
                player.transform.position = new Vector2(targetPos.x, targetPos.y);

                Debug.Log($"{player.PlayerName} moved to {targetPos.x},{targetPos.y}");

                // Következő játékos
                int nextIndex = (player.ID % PlayerCount);
                ActivePlayer = players[nextIndex];
                Debug.Log($"Next player: {ActivePlayer.PlayerName}");
            }
        }

        else
        {
            Debug.Log($"{player.PlayerName} cannot move diagonally or more than one tile!");
        }
    }

}
