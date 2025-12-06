using UnityEngine;

public class Tile : MonoBehaviour
{
    public Position GridPos;
    private SpriteRenderer _renderer;
    [SerializeField] private Color _baseColor;
    public Color BaseColor
    {
        get => _baseColor;
        set
        {
            _baseColor = value;
            if (_renderer != null)
            {
                _renderer.color = _baseColor;
                CurrentColor = _baseColor;
            }
        }
    }

    public Color CurrentColor { get; private set; }

    public Player Owner { get; private set; } = null;

    public Player SpecialOwner { get; private set; } = null;
    public Player SpecialCapturedBy { get; set; } = null;
    public bool IsSpecialTile => SpecialOwner != null;

    public bool IsBoostTile { get; private set; } = false;
    public bool BoostUsed { get; private set; } = false;
    private Color boostColor = new Color(1f, 1f, 0.6f, 1f); // halványsárga

    public void SetBoostTile()
    {
        IsBoostTile = true;
        BaseColor = boostColor;
    }

    public void ApplyBoost(Player player)
    {
        if (IsBoostTile && !BoostUsed)
        {
            player.BattleMultiplier += 0.3f; // kisebb boost mint special tile
            BoostUsed = true;
            Debug.Log($"{player.PlayerName} got boost from a boost tile!");
            ResetAppearanceToBase(); // lehet visszaállítjuk alap színre
        }
    }


    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
            _baseColor = _renderer.color;
        CurrentColor = _renderer.color;
    }

    public void Initialize(int x, int y)
    {
        GridPos = new Position(x, y);
        name = $"Tile ({x},{y})";
    }

    public void SetSpecial(Color color, Player owner)
    {
        SpecialOwner = owner;
        BaseColor = color;
    }

    public void HighlightUnderPlayer(Color playerColor)
    {
        if (_renderer != null)
        {
            Color faded = new Color(
                (_baseColor.r + playerColor.r) / 2f,
                (_baseColor.g + playerColor.g) / 2f,
                (_baseColor.b + playerColor.b) / 2f,
                1f
            );

            _renderer.color = faded;
            CurrentColor = faded;
        }
    }

    public void SetPlayerColor(Color color)
    {
        if (_renderer != null)
        {
            Color c = new Color(color.r, color.g, color.b, 0.5f);
            _renderer.color = c;
            CurrentColor = c;
        }
    }

    public void SetOwner(Player player)
    {
        Owner = player;
        SetPlayerColor(player.Color);
    }

    public bool IsOccupied()
    {
        return Owner != null;
    }

    public void ResetAppearanceToBase()
    {
        if (_renderer != null)
        {
            _renderer.color = _baseColor;
            CurrentColor = _baseColor;
        }
    }

    public void RestoreSpecial(Player owner, Player capturedBy = null)
    {
        SpecialOwner = owner;
        SpecialCapturedBy = capturedBy;

        if (capturedBy != null)
        {
            SetOwner(capturedBy);
        }
        else if (owner != null)
        {
            BaseColor = owner.Color;
        }
        else
        {
            ResetAppearanceToBase();
        }
    }

}
