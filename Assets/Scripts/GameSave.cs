[System.Serializable]
public class GameSave
{
    // ✅ Pálya és játék állapot
    public int boardSize;
    public int round;
    public int winScore;

    // ✅ Játékosok
    public PlayerData[] players;

    // ✅ Tile-ok
    public TileData[] tiles;

    // ✅ Aktív játékos neve
    public string activePlayerName;

    // =========================
    // ===== PLAYER DATA ======
    // =========================
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

    // =========================
    // ===== TILE DATA =========
    // =========================
    [System.Serializable]
    public class TileData
    {
        public int x;
        public int y;

        // ✅ Tulajdonosok
        public string ownerName;
        public string specialOwnerName;
        public string specialCapturedBy;

        // ✅ Szín visszaállításhoz
        public float colorR;
        public float colorG;
        public float colorB;

        // ✅ BOOST TILE MENTÉS
        public bool isBoostTile;
        public bool boostUsed;
    }
}
