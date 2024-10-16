using UnityEngine;

namespace MiniGames.GuessColor
{
    public class Constants
    {
        public static int MAX_COLOR_NUM = 8;
        public static int MIN_COLOR_NUM = 6;

        public static int MAX_GUESS_NUM = 6;
        public static int MIN_GUESS_NUM = 4;

        public static int MAX_ATTEMPT_NUM = 12;
        public static int MIN_ATTEMPT_NUM = 5;

        public static Color[] COLORS =
        {
            new Color(0.95f, 0.61f, 0.07f), // 温暖的橙色 (橙黄)
            new Color(0.36f, 0.72f, 0.36f), // 柔和的绿色 (淡绿)
            new Color(0.25f, 0.47f, 0.85f), // 静谧的蓝色 (淡蓝)
            new Color(0.91f, 0.30f, 0.24f), // 鲜艳的红色 (朱红)
            new Color(0.68f, 0.47f, 0.90f), // 浅紫色 (淡紫)
            new Color(1.00f, 1.00f, 0.40f), // 松花 (黄色系)
            new Color(0.68f, 0.82f, 0.93f), // 碧落 (青色系)
            new Color(0.96f, 0.75f, 0.78f), // 桃矢 (粉丝系)
        };

        public static Color[] RES_COLORS =
        {
            new Color(0.70f, 0.70f, 0.70f), // 都不对
            new Color(1.00f, 0.94f, 0.00f), // 颜色对
            new Color(0.11f, 1.00f, 0.00f), // 都对
        };
    }
}
