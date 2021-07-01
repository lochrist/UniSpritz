using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniMini
{
    class SpriteCache
    {

    }

    struct SpriteId
    {
        int id;
    }

    class Layer
    {
        private SpriteCache cache;
        private Material m_Material;
        private float m_MinScale = 0.15f;
        private float m_MaxScale = 0.2f;
        private int m_SpriteCount;
        private Mesh m_Mesh;

        // Matrix here is a compressed transform information
        // xy is the position, z is rotation, w is the scale
        private ComputeBuffer m_TransformBuffer;
        // uvBuffer contains float4 values in which xy is the uv dimension and zw is the texture offset
        private ComputeBuffer m_UvBuffer;
        private ComputeBuffer m_ColorBuffer;
        private ComputeBuffer m_DrawArgsBuffer;
        private uint[] m_DrawArgs;

        Vector4[] m_Transforms;
        Vector4[] m_Uvs;
        Vector4[] m_Colors;

        public void DrawSprite(int x, int y, SpriteId id)
        {

        }

        public void Draw()
        {

        }
    }

    class Renderer
    {
        Layer[] layers;
    }
    
    public static class Spritz
    {
        static Camera m_Camera;
        static SpritzGameComponent m_GameBox;
        static SpritzGame m_Game;
        static List<Layer> m_Layers;

        public static void Initialize(GameObject root, SpritzGame game)
        {
            m_Layers = new List<Layer>();
            m_Camera = root.AddComponent<Camera>();
            m_GameBox = root.AddComponent<SpritzGameComponent>();
            m_Game = game;

            m_Camera.clearFlags = CameraClearFlags.Nothing;
            m_Camera.cullingMask = 0;
            m_Camera.nearClipPlane = 0.0f;
            m_Camera.farClipPlane = 10.0f;
            m_Camera.depth = 100;
            m_Camera.orthographic = true;

            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;
        }

        internal static void Update()
        {
            m_Camera.orthographicSize = Screen.height * 0.5f;
            m_Game.Update();
        }

        internal static void Render()
        {
            m_Game.Render();
        }

        internal static void RenderLayers()
        {
            foreach (var l in m_Layers)
                l.Draw();
        }
    }
}

