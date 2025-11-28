using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    private const int BoardSize = 4;

    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private UIManager uiManager;

    public int PlayerCount = 2;

    private Tile[,] Board = new Tile[BoardSize, BoardSize];
    private Player[] players;

    private Color[] PlayerColors = { Color.red, Color.blue, Color.green, Color.magenta };
    public Player ActivePlayer { get; private set; }

    void Start()
    {
        PlayerCount = MenuManager.PlayerCountSelected;
        GenerateGrid();
        PlacePlayersInCorners();
        PlaceSpecialTilesForPlayers();

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

        if (!((dx == 1 && dy == 0) || (dx == 0 && dy == 1)))
        {
            Debug.Log($"{player.PlayerName} cannot move diagonally or more than one tile!");
            return;
        }

        Tile targetTile = Board[targetPos.x, targetPos.y];
        if (targetTile == null) return;

        HandleSpecialTileVisit(player, targetTile);

        if (targetTile.Owner == player)
        {
            player.CurrentPosition = targetPos;
            player.transform.position = new Vector2(targetPos.x, targetPos.y);

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
            player.transform.position = new Vector2(targetPos.x, targetPos.y);

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
            player.transform.position = new Vector2(targetPos.x, targetPos.y);
        }

        NextPlayerTurn(player);
    }


    private void CheckWinCondition(Player player)
    {
        if (player.Score >= 12)
        {
            Debug.Log($"{player.PlayerName} reached 12 points and WINS the game!");

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

        if (uiManager != null)
            uiManager.UpdateActivePlayer(ActivePlayer);
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
                startTile.SetOwner(player);
                startTile.HighlightUnderPlayer(PlayerColors[i]);
            }
        }
    }


    private void HandleSpecialTileVisit(Player visitor, Tile tile)
    {
        if (!tile.IsSpecialTile) return;

        Player originalOwner = tile.SpecialOwner;

        Debug.Log($"{visitor.PlayerName} stepped on special tile owned by {(originalOwner != null ? originalOwner.PlayerName : "none")}");


        if (originalOwner == visitor)
        {

            tile.SpecialCapturedBy = visitor;


            if (visitor.CanReceiveSpecialBuff && !visitor.HasBuff)
            {
                visitor.BattleMultiplier = 1.3f;
                visitor.HasBuff = true;
                Debug.Log($"{visitor.PlayerName} captured their own special tile and gained 1.3× battle multiplier.");
            }
            else
            {

                if (!visitor.CanReceiveSpecialBuff)
                    Debug.Log($"{visitor.PlayerName} stepped on their own special but had been preempted earlier — no buff.");
                else if (visitor.HasBuff)
                    Debug.Log($"[ALREADY BUFFED] {visitor.PlayerName} already has buff, stepping again does nothing.");
            }

            return;
        }

        tile.SpecialCapturedBy = visitor;


        if (originalOwner != null && originalOwner.CanReceiveSpecialBuff)
        {
            originalOwner.CanReceiveSpecialBuff = false;
            originalOwner.HasBuff = false;
            originalOwner.BattleMultiplier = 1.0f;

            Debug.Log($"{visitor.PlayerName} stepped on {originalOwner.PlayerName}'s special tile — {originalOwner.PlayerName} can no longer get the buff.");
        }


        Tile visitorsOwnSpecial = FindTileBySpecialOwner(visitor);
        if (visitorsOwnSpecial != null && visitorsOwnSpecial.SpecialCapturedBy == originalOwner)
        {

            visitor.BattleMultiplier = 1.0f;
            visitor.HasBuff = false;
            visitor.CanReceiveSpecialBuff = false;

            if (originalOwner != null)
            {
                originalOwner.BattleMultiplier = 1.0f;
                originalOwner.HasBuff = false;
                originalOwner.CanReceiveSpecialBuff = false;
            }

            Debug.Log($"Reciprocal special capture between {visitor.PlayerName} and {originalOwner.PlayerName}: battle multipliers set to 1.0.");
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
}
