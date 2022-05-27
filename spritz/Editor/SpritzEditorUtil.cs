using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace UniMini
{
    public class SpritzEditorUtil
    {
        public static void AddTransparency(Texture2D src)
        {
            var buffer = src.GetPixels32();
            for (var i = 0; i < buffer.Length; ++i)
            {
                Color32 c = buffer[i];
                if (c.r == 0 && c.g == 0 && c.b == 0 && c.a == 255)
                {
                    buffer[i] = new Color32(0, 0, 0, 0);
                }
            }
            src.SetPixels32(buffer);
            src.Apply();
        }

        public static void AddTransparency(string src, string output)
        {
            var srcTex = AssetDatabase.LoadAssetAtPath<Texture2D>(src);
            if (!srcTex)
                throw new System.Exception($"Invalid texture path: {src}");

            AddTransparency(srcTex);

            var bytes = srcTex.EncodeToPNG();
            File.WriteAllBytes(output, bytes);
        }

        [MenuItem("Spritz/Add Transparency to")]
        static void AddTransparencyTo()
        {
            AddTransparency("Assets/Resources/Spritesheets/particle-system.png", "Assets/Resources/Spritesheets/particle-system.png");
        }
    }
}
