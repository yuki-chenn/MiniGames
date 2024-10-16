using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace MiniGames.DoodleJump
{
    class Constants
    {
        public static float VELOCITY_EPS = 0.2f;

        public static float MIN_SPAWN_HEIGHT = 0.5f;
        public static float MAX_SPAWN_HEIGHT = 2.5f;

        public static Vector2 FIRST_PLATFORM_POS = new Vector2(2.5f, -6.5f);

        public static float SPAWN_BROKENPLATFORM_RATE = 0.2f;
        public static float SPAWN_SPRING_RATE = 0.05f;
        public static float SPAWN_ROCKET_RATE = 0.01f;

        public static float ROCKET_TIME = 3f;

        public static Vector2 PLAYER_INIT_POS = new Vector2(0, -7f);

        public static float ITEM_Y_OFFSET = 0.11f;

    }
}
