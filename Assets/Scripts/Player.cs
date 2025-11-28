using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerName;
    public int ID;
    public Position CurrentPosition;

    private SpriteRenderer _renderer;
    public Color Color { get; private set; }

    public int Score { get; set; } = 0;


    public float BattleMultiplier = 1.0f;
    public bool CanReceiveSpecialBuff = true;
    public bool HasBuff = false;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int id, string name, Position startPos, Color color)
    {
        ID = id;
        PlayerName = name;
        CurrentPosition = startPos;
        Color = color;

        transform.position = new Vector2(startPos.x, startPos.y);

        if (_renderer != null)
            _renderer.color = color;
    }
}
