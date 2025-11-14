using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerName;
    public int ID;
    public Position CurrentPosition;

    public void Initialize(int id, string name, Position startPos)
    {
        ID = id;
        PlayerName = name;
        CurrentPosition = startPos;
        transform.position = new Vector2(startPos.x, startPos.y);
    }
}
