using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UniMini
{
    public static class Vector2Extensions
    {
        public static float Dot(this Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y; 
        }

        public static float Cross(this Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }

        public static Vector2 Cross(this Vector2 a, float s)
        {
            return new Vector2(s * a.y, -s * a.x);
        }

        public static Vector2 Cross(float s, Vector2 a)
        {
            return new Vector2(-s * a.y, s * a.x);
        }

        public static Vector2 Abs(this Vector2 v)
        {
            return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        }

        public static Vector2 AddScalarWithAngle(this Vector2 v, float d, float angle)
        {
            return new Vector2(v.x + d * Mathf.Cos(angle), v.y + d * Mathf.Sin(angle));
        }
    }

    public struct Matrix2x2
    {
        public Vector2 col1;
        public Vector2 col2;

        public Matrix2x2(float angle)
        {
            float c = Mathf.Cos(angle);
            float s = Mathf.Sin(angle);
            col1.x = c; 
            col2.x = -s;
            col1.y = s; 
            col2.y = c;
        }

        public Matrix2x2(Vector2 col1, Vector2 col2)
        {
            this.col1 = col1;
            this.col2= col2;
        }

        public Matrix2x2 Transpose()
        {
            return new Matrix2x2(new Vector2(col1.x, col2.x), new Vector2(col1.y, col2.y));
        }

        public Matrix2x2 Invert()
        {
            var a = col1.x;
            var b = col2.x;
            var c = col1.y;
            var d = col2.y;
            var bMat = new Matrix2x2();
            var det = a * d - b * c;
            det = 1.0f / det;
            bMat.col1.x = det * d;
            bMat.col2.x = -det * b;
            bMat.col1.y = -det * c;
            bMat.col2.y = det * a;
            return bMat;
        }

        public Matrix2x2 Abs()
        {
            return new Matrix2x2(col1.Abs(), col2.Abs());
        }

        public static Vector2 operator*(Matrix2x2 m, Vector2 v)
        {
            return new Vector2(m.col1.x * v.x + m.col2.x * v.y, m.col1.y * v.x + m.col2.y * v.y);
        }

        public static Matrix2x2 operator *(Matrix2x2 m1, Matrix2x2 m2)
        {
            return new Matrix2x2(m1 * m2.col1, m1 * m2.col2);
        }

        public static Matrix2x2 operator +(Matrix2x2 m1, Matrix2x2 m2)
        {
            return new Matrix2x2(m1.col1 + m2.col1, m1.col2 + m2.col2);
        }
    }

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

        public static void DrawTimeInfo(int x = 0, int y = 0, Font font = null)
        {
            var fps = Mathf.RoundToInt(Spritz.frame / Time.time);
            var msg = $"t:{Time.time:F1} f:{Spritz.frame} fps:{fps}";
            if (font != null)
                Spritz.Print(font, msg, x, y, Color.white);
            else
                Spritz.Print(msg, x, y, Color.white);
        }

        public static bool Approximately(float a, float b, float epsilon = 0.000001f)
        {
            return ((a > b) ? (a - b) : (b - a)) <= epsilon;
        }
    }
}

