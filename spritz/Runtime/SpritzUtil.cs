using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UniMini
{
    public static class SpritzUtil
    {
        public static bool debugLogEnabled = true;
        public static string debugLogDir = "Assets/Logs";
        public static string debugLogFile = $"{debugLogDir}/spritz.log";

        public static void Debug(params string[] logs)
        {
            if (!debugLogEnabled)
                return;

            if (!Directory.Exists(debugLogDir))
            {
                Directory.CreateDirectory(debugLogDir);
            }

            using (var stream = File.AppendText(debugLogFile))
            {
                foreach (var l in logs)
                    stream.Write(l);
                stream.WriteLine();
            }
        }

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
            var fps = Mathf.RoundToInt(Spritz.frame / Time.time);
            var msg = $"t:{Time.time:F1} f:{Spritz.frame} fps:{fps}";
            Spritz.Print(msg, x, y, Color.white);
        }

        public static bool Approximately(float a, float b, float epsilon = 0.000001f)
        {
            return ((a > b) ? (a - b) : (b - a)) <= epsilon;
        }
    }
}

