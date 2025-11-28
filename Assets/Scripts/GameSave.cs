
[System.Serializable]
public class GameSave
{
    public PlayerData[] players;
    public TileData[] tiles;
    public string activePlayerName;

    [System.Serializable]
    public class PlayerData
    {
        public string playerName;
        public int id;
        public int posX;
        public int posY;
        public int score;
        public float battleMultiplier;
        public bool hasBuff;
        public bool canReceiveSpecialBuff;
        public float colorR;
        public float colorG;
        public float colorB;
    }

    [System.Serializable]
    public class TileData
    {
        public int x;
        public int y;
        public string ownerName;
        public string specialOwnerName;
        public string specialCapturedBy;
        public float colorR;
        public float colorG;
        public float colorB;
    }
}
