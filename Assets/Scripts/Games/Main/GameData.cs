using System;

namespace MiniGames.Main
{
    [Serializable]
    public class GameData
    {
        public int gemCount;
        public BW_GameData bwGameData;
        public DJ_GameData djGameData;

        public GameData()
        {
            gemCount = 0;
            bwGameData = new BW_GameData();
            djGameData = new DJ_GameData();
        }

    }

    [Serializable]
    public class BW_GameData
    {
        public int bestScore;

        public BW_GameData()
        {
            bestScore = 0;
        }
    }

    [Serializable]
    public class DJ_GameData
    {
        public int bestScore;

        public DJ_GameData()
        {
            bestScore = 0;
        }
    }
}
