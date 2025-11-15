using UnityEngine;

public class Tile : MonoBehaviour
{
    public Position GridPos;
    private SpriteRenderer _renderer;
    private Color _baseColor; // eredeti szín (special vagy default)

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

    public void SetSpecial(Color color)
    {
        if (_renderer != null)
        {
            _renderer.color = color;
            _baseColor = color; // tároljuk az alap színt
        }
    }

    // highlight: hozzáadja a player színét az alap színhez
    public void HighlightUnderPlayer(Color playerColor)
    {
        if (_renderer != null)
        {
            Color faded = new Color(
                (_baseColor.r + playerColor.r) / 2f,
                (_baseColor.g + playerColor.g) / 2f,
                (_baseColor.b + playerColor.b) / 2f,
                1f // teljes opacity
            );
            _renderer.color = faded;
        }
    }

    public void SetPlayerColor(Color color)
    {
        if (_renderer != null)
        {
            // Alapszín → a player halvány színe
            _renderer.color = new Color(color.r, color.g, color.b, 0.5f);
        }
    }

}
