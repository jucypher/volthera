using UnityEngine;

public class Tile : MonoBehaviour
{
    public Position GridPos;
    private SpriteRenderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Inicializálás: koordináták
    public void Initialize(int x, int y)
    {
        GridPos = new Position(x, y);
        name = $"Tile ({x},{y})"; // Hierarchy-ben könnyebb látni
    }

    // Special tile beállítása (sárga szín)
    public void SetSpecial(string species)
    {
        if (_renderer != null)
            _renderer.color = Color.yellow;

        // Később ide lehet tenni: this.species = species;
    }
}
