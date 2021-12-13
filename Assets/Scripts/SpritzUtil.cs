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
    }
}

