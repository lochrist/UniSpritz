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


    public static class Spritz
    {
        static Camera m_Camera;
        static SpritzGameComponent m_GameBox;
        static SpritzGame m_Game;
        static List<Layer> m_Layers;
        static Layer currentLayer => m_Layers[m_CurrentLayerId];
        static int m_CurrentLayerId; 

        public static void Initialize(GameObject root, SpritzGame game)
        {
            m_Layers = new List<Layer>(4);
            // m_Camera = root.AddComponent<Camera>();
            m_Camera = root.GetComponent<Camera>();
            m_GameBox = root.AddComponent<SpritzGameComponent>();
            m_Game = game;

            /*
            m_Camera.clearFlags = CameraClearFlags.Nothing;
            m_Camera.cullingMask = 0;
            m_Camera.nearClipPlane = 0.0f;
            m_Camera.farClipPlane = 10.0f;
            m_Camera.depth = 100;
            m_Camera.orthographic = true;
            m_Camera.orthographicSize = 5;
            */

            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;

            game.Initialize();
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
        #endregion

        #region Internals
        internal static void Update()
        {
            // m_Camera.orthographicSize = Screen.height * 0.5f;
            m_Game.Update();
        }

        internal static void Render()
        {
            foreach (var l in m_Layers)
                l.PreRender();
            m_Game.Render();
        }

        internal static void RenderLayers()
        {
            foreach (var l in m_Layers)
                l.Render();
        }

        internal static void CleanUp()
        {
            foreach (var l in m_Layers)
                l.Release();
        }
        #endregion

        private static int CreateLayer(SpriteSheet spriteSheet)
        {
            var layer = new Layer(spriteSheet);
            m_Layers.Add(layer);
            currentLayerId = m_Layers.Count - 1;
            return currentLayerId;
        }
    }
}

