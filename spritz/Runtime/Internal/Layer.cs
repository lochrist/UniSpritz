using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniMini
{
    public interface Layer
    {
        void Clear(Color32 c);
        Color32 GetPixel(int x, int y);
        void DrawPixel(int x, int y, Color32 c);
        void DrawPixels(int x, int y, int width, int height, Color32[] c);
        void DrawPixels(int x, int y, int width, int height, Color32 c);
        void DrawSprite(SpriteId id, int x, int y);
        void DrawSprite(SpriteId id, int x, int y, bool flipX, bool flipY, float angle);
        SpriteDesc[] GetSpriteDescs();
        SpriteDesc GetSpriteDesc(SpriteId id);
        void PreRender();
        void Render();
        void CleanUp();
    }

    class ComputeLayer : Layer
    {
        private SpriteSheet m_Sheet;
        private Material m_Material;
        private int count => m_Transforms.Count;
        private Mesh m_Mesh;

        // Matrix here is a compressed transform information
        // xy is the position, z is rotation, w is the scale
        private ComputeBuffer m_TransformBuffer;
        // uvBuffer contains float4 values in which xy is the uv dimension and zw is the texture offset
        private ComputeBuffer m_UvBuffer;
        private ComputeBuffer m_ColorBuffer;
        private ComputeBuffer m_DrawArgsBuffer;
        private uint[] m_DrawArgs;
        private int m_UvBufferId;
        private int m_ColorBufferId;
        private int m_TransformBufferId;
        private int m_LayerIndex;
        private Vector4 m_SinglePixelUV;

        private static readonly Bounds m_Bounds = new Bounds(Vector2.zero, Vector3.one);

        List<Vector4> m_Uvs;
        List<Vector4> m_Transforms;
        List<Vector4> m_Colors;

        public ComputeLayer(SpritzGame game, SpriteSheet sheet, int layerIndex, int bufferCacheHint = 512)
        {
            m_Sheet = sheet;
            var shader = Shader.Find("Custom/Spritz");
            m_Material = new Material(shader);
            m_Material.mainTexture = sheet.texture;
            m_Uvs = new List<Vector4>(bufferCacheHint);
            m_Transforms = new List<Vector4>(bufferCacheHint);
            m_Colors = new List<Vector4>(bufferCacheHint);
            m_DrawArgs = new uint[] {
                6, (uint)count, 0, 0, 0
            };

            m_UvBufferId = Shader.PropertyToID("uvBuffer");
            m_ColorBufferId = Shader.PropertyToID("colorsBuffer");
            m_TransformBufferId = Shader.PropertyToID("transformBuffer");
            m_LayerIndex = layerIndex;
            m_Mesh = Utils.CreateQuad((float)m_LayerIndex);

            // TODO: need to generate an empty pixel at a proper coord.
            m_SinglePixelUV = SpriteSheet.ComputeUV(sheet, new Rect(0, 511, 1, 1));
        }

        #region LayerAPI
        public void Clear(Color32 c)
        {

        }

        public Color32 GetPixel(int x, int y)
        {
            return new Color32(0,0,0,255);
        }

        public void DrawPixel(int x, int y, Color32 c)
        {
            m_Uvs.Add(m_SinglePixelUV);

            float rotation = 0f;
            float scale = 1 / Spritz.pixelPerUnit;
            m_Transforms.Add(new Vector4(x/ Spritz.pixelPerUnit, y/ Spritz.pixelPerUnit, rotation, scale));

            m_Colors.Add(new Vector4(c.r, c.g, c.b, c.a));
        }

        public void DrawPixels(int x, int y, int width, int height, Color32[] c)
        {
            throw new System.NotImplementedException();
        }

        public void DrawPixels(int x, int y, int width, int height, Color32 c)
        {
            throw new System.NotImplementedException();
        }

        public void DrawSprite(SpriteId id, int x, int y)
        {
            var sprite = m_Sheet.GetSpriteById(id);
            m_Uvs.Add(sprite.uv);

            float rotation = 0f;
            float scale = 1f;
            m_Transforms.Add(new Vector4(x/ Spritz.pixelPerUnit, y / Spritz.pixelPerUnit, rotation, scale));

            var c = Color.white;
            m_Colors.Add(new Vector4(c.r, c.g, c.b, c.a));
        }

        public void DrawSprite(SpriteId id, int x, int y, bool flipX, bool flipY, float angle)
        {
            throw new System.NotImplementedException();
        }

        public SpriteDesc[] GetSpriteDescs()
        {
            return m_Sheet.spriteDescriptors;
        }

        public SpriteDesc GetSpriteDesc(SpriteId id)
        {
            return m_Sheet.GetSpriteById(id);
        }

        public void PreRender()
        {
            m_Uvs.Clear();
            m_Transforms.Clear();
            m_Colors.Clear();
        }
        
        public void Render()
        {
            m_DrawArgs[1] = (uint)count;
            if (m_DrawArgs[1] == 0)
            {
                CleanUp();
                return;
            }

            if (m_TransformBuffer == null || m_TransformBuffer.count < count)
            {
                CleanUp();
                m_TransformBuffer = new ComputeBuffer(count, 16);
                m_UvBuffer = new ComputeBuffer(count, 16);
                m_ColorBuffer = new ComputeBuffer(count, 16);
                m_DrawArgsBuffer = new ComputeBuffer(1, m_DrawArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            }

            m_UvBuffer.SetData(m_Uvs);
            m_Material.SetBuffer(m_UvBufferId, m_UvBuffer);

            m_ColorBuffer.SetData(m_Colors);
            m_Material.SetBuffer(m_ColorBufferId, m_ColorBuffer);

            m_TransformBuffer.SetData(m_Transforms);
            m_Material.SetBuffer(m_TransformBufferId, m_TransformBuffer);
            
            m_DrawArgsBuffer.SetData(m_DrawArgs);

            Graphics.DrawMeshInstancedIndirect(m_Mesh, 0, m_Material, m_Bounds, m_DrawArgsBuffer);
        }

        public void CleanUp()
        {
            if (m_TransformBuffer != null)
            {
                m_TransformBuffer.Release();
                m_TransformBuffer = null;

                m_UvBuffer.Release();
                m_UvBuffer = null;

                m_ColorBuffer.Release();
                m_ColorBuffer = null;

                m_DrawArgsBuffer.Release();
                m_DrawArgsBuffer = null;
            }
        }
        #endregion
    }
}