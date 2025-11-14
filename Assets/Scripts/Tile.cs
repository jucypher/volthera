using UnityEngine;

public class Tile : MonoBehaviour
{
    public Position GridPos;
    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int x, int y)
    {
        GridPos = new Position(x, y);
        name = $"Tile ({x},{y})";
    }

    public void SetSpecial(string species)
    {
        if (_renderer != null)
            _renderer.color = Color.yellow; // special tile sárga
    }
}
