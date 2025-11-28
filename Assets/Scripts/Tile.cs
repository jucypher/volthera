using UnityEngine;

public class Tile : MonoBehaviour
{
    public Position GridPos;
    private SpriteRenderer _renderer;
    private Color _baseColor;

    public Player Owner { get; private set; } = null;


    public Player SpecialOwner { get; private set; } = null;
    public Player SpecialCapturedBy { get; set; } = null;
    public bool IsSpecialTile => SpecialOwner != null;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
            _baseColor = _renderer.color;
    }

    public void Initialize(int x, int y)
    {
        GridPos = new Position(x, y);
        name = $"Tile ({x},{y})";
    }

    public void SetSpecial(Color color, Player owner)
    {
        SpecialOwner = owner;

        if (_renderer != null)
        {
            _renderer.color = color;
            _baseColor = color;
        }
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
        }
    }

    public void SetPlayerColor(Color color)
    {
        if (_renderer != null)
        {
            _renderer.color = new Color(color.r, color.g, color.b, 0.5f);
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
}
