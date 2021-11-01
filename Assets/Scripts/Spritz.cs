using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    public struct SpriteId
    {
        public int value;

        public SpriteId(string name)
        {
            value = name.GetHashCode();
        }

        public SpriteId(int id)
        {
            value = id;
        }

        public bool isValid => value != 0;

        public bool Equals(SpriteId other)
        {
            return value == other.value;
        }

        public override bool Equals(object other)
        {
            return value == ((SpriteId)other).value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static bool operator ==(SpriteId a, SpriteId b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(SpriteId a, SpriteId b)
        {
            return a.value != b.value;
        }
    }

    public enum LayerType
    {
        Compute,
        Texture
    }

    public static class Spritz
    {
        static Camera m_Camera;
        static SpritzGame m_Game;
        static List<Layer> m_Layers;
        static Layer currentLayer => m_Layers[m_CurrentLayerId];
        static int m_CurrentLayerId;

        internal static float zoom;
        internal static float pixelPerUnit => m_Game.pixelPerUnit;
        internal static float unitsPerPixel;
        internal static Vector2 cameraOffset;

        public static void Initialize(GameObject root, SpritzGame game, Camera camera = null)
        {
            m_Layers = new List<Layer>(4);
            
            m_Game = game;
            camera = root.GetComponent<Camera>();
            if (camera == null)
            {
                m_Camera = root.AddComponent<Camera>();
            }
            
            m_Camera = camera;
            SetupCamera();

            root.transform.position = new Vector3(0f, 0f, -10f);
            root.transform.rotation = Quaternion.identity;

            game.InitializeSpritz();
        }

        static void SetupCamera()
        {
            m_Camera.transform.position = new Vector3(0f, 0f, -10f);
            m_Camera.transform.rotation = Quaternion.identity;

            m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.backgroundColor = Color.black;
            m_Camera.nearClipPlane = 0.3f;
            m_Camera.farClipPlane = 1000.0f;
            m_Camera.orthographic = true;

            // zoom level (PPU scale)
            int verticalZoom = Screen.height / m_Game.resolution.y;
            int horizontalZoom = Screen.width / m_Game.resolution.y;
            float zoom = Math.Max(1, Math.Min(verticalZoom, horizontalZoom));
            float pixelHeight = Screen.height;
            float orthoSize = (pixelHeight * 0.5f) / (zoom * m_Game.pixelPerUnit);
            unitsPerPixel = 1.0f / (zoom * m_Game.pixelPerUnit);

            m_Camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            m_Camera.orthographicSize = orthoSize;
        }

        #region Layers
        public static int CreateLayer(string spriteSheetName)
        {
            return CreateLayer(SpriteSheet.CreateFromResource(spriteSheetName));
        }

        public static int CreateLayer(Sprite[] sprites)
        {
            return CreateLayer(SpriteSheet.CreateFromSprites(sprites));
        }

        public static SpriteId[] GetSprites(int layer)
        {
            return m_Layers[layer].GetSprites();
        }

        public static int currentLayerId
        {
            get => m_CurrentLayerId;
            set 
            {
                m_CurrentLayerId = value;
                if (value >= m_Layers.Count || value < 0)
                    m_CurrentLayerId = 0;
            }
        }
        #endregion

        #region Draw
        public static void DrawSprite(SpriteId id, int x, int y)
        {
            currentLayer.DrawSprite(id, x, y);
        }

        public static void Circle(int x, int y, int radius, Color color, bool fill)
        {

        }

        public static void Ellipse(int x0, int y0, int x1, int y1, Color color, bool fill)
        {

        }

        public static void Line(int x0, int y0, int x1, int y1, Color color)
        {

        }

        public static void Rectangle(int x0, int y0, int x1, int y1, Color color)
        {

        }

        public static RectInt Clip(int x0, int y0, int x1, int y1)
        {
            return new RectInt();
        }

        public static void Print(string text, int x, int y, Color color)
        {

        }

        public static Vector2Int PrintSize(string text)
        {
            return new Vector2Int();
        }

        public static void DrawPixel(int x, int y, Color color)
        {
            currentLayer.DrawPixel(x, y, color);
        }
        #endregion

        #region Input
        // Use Event ?
        #endregion

        #region Sound
        public static void PlayMusic(AudioClip clip, float volume)
        {

        }

        public static void StopMusic(AudioClip clip)
        {
        }

        public static void PlaySound(AudioClip clip, float volume)
        {
        }

        public static void StopSound()
        {
        }
        #endregion

        #region Internals
        internal static void Update()
        {
            m_Game.UpdateSpritz();
        }

        internal static void Render()
        {
            foreach (var l in m_Layers)
                l.PreRender();
            m_Game.DrawSpritz();
        }

        internal static void RenderLayers()
        {
            for(var i = m_Layers.Count - 1; i >= 0; --i)
                m_Layers[i].Render();
        }

        internal static void CleanUp()
        {
            foreach (var l in m_Layers)
                l.CleanUp();
        }

        private static int CreateLayer(SpriteSheet spriteSheet)
        {
            var layer = (Layer)(m_Game.layerType == LayerType.Compute ? new ComputeLayer(m_Game, spriteSheet, m_Layers.Count) : new TextureLayer(m_Game, spriteSheet, m_Layers.Count));
            m_Layers.Add(layer);
            currentLayerId = m_Layers.Count - 1;
            return currentLayerId;
        }
        #endregion
    }
}