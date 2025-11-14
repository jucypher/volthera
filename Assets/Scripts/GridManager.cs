using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private const int BoardSize = 4;
    [SerializeField] private GameObject _tilePrefab;

    // Hány játékos van → ennyi special tile lesz
    public int PlayerCount = 2;

    private Tile[,] Board = new Tile[BoardSize, BoardSize];

    void Start()
    {
        GenerateGrid();
        PlaceSpecialTilesForPlayers();
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

    private void PlaceSpecialTilesForPlayers()
    {
        System.Random random = new System.Random();
        HashSet<Position> occupiedPositions = new HashSet<Position>();

        // A sarok koordináták, ahová nem kerülhet special tile
        Position[] corners = new Position[]
        {
            new Position(0,0),
            new Position(0,BoardSize-1),
            new Position(BoardSize-1,0),
            new Position(BoardSize-1,BoardSize-1)
        };

        for (int i = 0; i < PlayerCount; i++)
        {
            Position pos;
            bool placed = false;

            while (!placed)
            {
                int x = random.Next(BoardSize);
                int y = random.Next(BoardSize);
                pos = new Position(x, y);

                // Ellenőrzés: nem sarok és nincs foglalva
                bool isCorner = false;
                foreach (var c in corners)
                    if (c.x == pos.x && c.y == pos.y) { isCorner = true; break; }

                if (!isCorner && !occupiedPositions.Contains(pos))
                {
                    Board[pos.x, pos.y].SetSpecial("Player " + (i + 1));
                    occupiedPositions.Add(pos);
                    placed = true;
                }
            }
        }
    }
}
