using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private const int BoardSize = 4;

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _playerPrefab;

    public int PlayerCount = 2; // 2–4 játékos

    private Tile[,] Board = new Tile[BoardSize, BoardSize];
    private Player[] players;

    // Játékos színek
    private Color[] PlayerColors = {
        Color.red,
        Color.blue,
        Color.green,
        Color.magenta
    };

    void Start()
    {
        GenerateGrid();
        PlaceSpecialTilesForPlayers();
        PlacePlayersInCorners();
    }

    // ---- GRID GENERATION ----
    void GenerateGrid()
    {
        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                GameObject t = Instantiate(
                    _tilePrefab,
                    new Vector2(x, y),
                    Quaternion.identity
                );

                t.transform.parent = transform;

                Tile tile = t.GetComponent<Tile>();
                tile.Initialize(x, y);

                Board[x, y] = tile;
            }
        }
    }

    // ---- SPECIAL TILE PLACEMENT ----
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

                // sarkok tiltása
                if ((x == 0 && y == 0) ||
                    (x == 0 && y == BoardSize - 1) ||
                    (x == BoardSize - 1 && y == 0) ||
                    (x == BoardSize - 1 && y == BoardSize - 1))
                {
                    continue;
                }

                if (!occupied.Contains(pos))
                {
                    Board[x, y].SetSpecial(PlayerColors[i]); // saját színű special tile
                    occupied.Add(pos);
                    placed = true;
                }
            }
        }
    }

    // ---- PLAYER SPAWN ----
    void PlacePlayersInCorners()
    {
        players = new Player[PlayerCount];

        Position[] cornerPositions;

        if (PlayerCount == 2)
        {
            // átlós sarkok
            cornerPositions = new Position[]
            {
                new Position(0,0),
                new Position(BoardSize - 1, BoardSize - 1)
            };
        }
        else if (PlayerCount == 3)
        {
            cornerPositions = new Position[]
            {
                new Position(0,0),
                new Position(0, BoardSize - 1),
                new Position(BoardSize - 1, BoardSize - 1)
            };
        }
        else
        {
            cornerPositions = new Position[]
            {
                new Position(0,0),
                new Position(0, BoardSize - 1),
                new Position(BoardSize - 1, 0),
                new Position(BoardSize - 1, BoardSize - 1)
            };
        }

        for (int i = 0; i < PlayerCount; i++)
        {
            GameObject pGO = Instantiate(
                _playerPrefab,
                new Vector2(cornerPositions[i].x, cornerPositions[i].y),
                Quaternion.identity
            );

            pGO.transform.parent = transform;

            Player playerScript = pGO.GetComponent<Player>();
            playerScript.Initialize(
                i + 1,
                "Player " + (i + 1),
                cornerPositions[i],
                PlayerColors[i]
            );

            players[i] = playerScript;
        }
    }
}
