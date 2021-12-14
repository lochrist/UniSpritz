using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public static class SpritzUtil
    {
        public static float Sinp8(float a)
        {
            // Pico-8 keeps its angle between 0-1
            return Mathf.Sin(-a * Mathf.PI * 2);
        }

        public static float Cosp8(float a)
        {
            // Pico-8 keeps its angle between 0-1
            // https://pico-8.fandom.com/wiki/Cos
            return Mathf.Cos(a * Mathf.PI * 2);
        }

        public static void DrawTimeInfo(int x = 0, int y = 0)
        {
            var fps = (int)(Spritz.frame / Time.time);
            var msg = $"t:{Time.time:F1} f:{Spritz.frame} fps:{fps}";
            Spritz.Print(msg, x, x, Color.white);
        }
    }
}
