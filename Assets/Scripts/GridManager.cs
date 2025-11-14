using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private const int BoardSize = 4;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _playerPrefab;
    public int PlayerCount = 2; // 2-4 fő
    private Tile[,] Board = new Tile[BoardSize, BoardSize];
    private Player[] players;

    private string[] Species = { "Zorg", "Vrelián", "Humanoid", "X'tar" };

    void Start()
    {
        GenerateGrid();
        PlaceSpecialTilesForPlayers();
        PlacePlayersInCorners();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < BoardSize; x++)
            for (int y = 0; y < BoardSize; y++)
            {
                GameObject t = Instantiate(_tilePrefab, new Vector2(x, y), Quaternion.identity);
                t.transform.parent = transform;
                Tile tile = t.GetComponent<Tile>();
                tile.Initialize(x, y);
                Board[x, y] = tile;
            }
    }

    void PlaceSpecialTilesForPlayers()
    {
        System.Random random = new System.Random();
        HashSet<Position> occupied = new HashSet<Position>();

        foreach (var species in Species)
        {
            if (occupied.Count >= PlayerCount) break; // csak PlayerCount special tile

            Position pos;
            bool placed = false;
            while (!placed)
            {
                int x = random.Next(BoardSize);
                int y = random.Next(BoardSize);
                pos = new Position(x, y);

                // sarok kihagyása
                if ((x == 0 && y == 0) || (x == 0 && y == BoardSize - 1) || (x == BoardSize - 1 && y == 0) || (x == BoardSize - 1 && y == BoardSize - 1))
                    continue;

                if (!occupied.Contains(pos))
                {
                    Board[x, y].SetSpecial(species);
                    occupied.Add(pos);
                    placed = true;
                }
            }
        }
    }

    void PlacePlayersInCorners()
    {
        players = new Player[PlayerCount];
        Position[] cornerPositions;

        if (PlayerCount == 2)
            cornerPositions = new Position[] { new Position(0, 0), new Position(BoardSize - 1, BoardSize - 1) };
        else if (PlayerCount == 3)
            cornerPositions = new Position[] { new Position(0, 0), new Position(0, BoardSize - 1), new Position(BoardSize - 1, BoardSize - 1) };
        else
            cornerPositions = new Position[] { new Position(0, 0), new Position(0, BoardSize - 1), new Position(BoardSize - 1, 0), new Position(BoardSize - 1, BoardSize - 1) };

        for (int i = 0; i < PlayerCount; i++)
        {
            GameObject pGO = Instantiate(_playerPrefab, new Vector2(cornerPositions[i].x, cornerPositions[i].y), Quaternion.identity);
            pGO.transform.parent = transform;
            Player playerScript = pGO.GetComponent<Player>();
            playerScript.Initialize(i + 1, "Player " + (i + 1), cornerPositions[i]);
            players[i] = playerScript;
        }
    }
}
