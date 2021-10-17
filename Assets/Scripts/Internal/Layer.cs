using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UniMini
{
    class Layer
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
        private float m_PixelPerUnit;
        private Vector4 m_SinglePixelUV;

        private static readonly Bounds m_Bounds = new Bounds(Vector2.zero, Vector3.one);

        List<Vector4> m_Uvs;
        List<Vector4> m_Transforms;
        List<Vector4> m_Colors;

        public Layer(SpriteSheet sheet, int layerIndex, float pixelPerUnit, int bufferCacheHint = 512)
        {
            m_PixelPerUnit = pixelPerUnit;
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
            m_Mesh = CreateQuad((float)m_LayerIndex);

            m_SinglePixelUV = SpriteSheet.ComputeUV(sheet, new Rect(0, 511, 1, 1));
        }

        public void DrawPixel(int x, int y, Color c)
        {
            m_Uvs.Add(m_SinglePixelUV);

            float rotation = 0f;
            float scale = 1 / m_PixelPerUnit;
            m_Transforms.Add(new Vector4(x/m_PixelPerUnit, y/m_PixelPerUnit, rotation, scale));

            m_Colors.Add(new Vector4(c.r, c.g, c.b, c.a));
        }

        public void DrawSprite(SpriteId id, int x, int y)
        {
            var sprite = m_Sheet.GetSpriteById(id);
            m_Uvs.Add(sprite.uv);

            float rotation = 0f;
            float scale = 1f;
            m_Transforms.Add(new Vector4(x/m_PixelPerUnit, y / m_PixelPerUnit, rotation, scale));

            var c = Color.white;
            m_Colors.Add(new Vector4(c.r, c.g, c.b, c.a));
        }

        internal SpriteId[] GetSprites()
        {
            return m_Sheet.spriteDescriptors.Select(sd => sd.id).ToArray();
        }

        internal void PreRender()
        {
            m_Uvs.Clear();
            m_Transforms.Clear();
            m_Colors.Clear();
        }
        
        internal void Render()
        {
            m_DrawArgs[1] = (uint)count;
            if (m_DrawArgs[1] == 0)
            {
                Release();
                return;
            }

            if (m_TransformBuffer == null || m_TransformBuffer.count < count)
            {
                Release();
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

        internal void Release()
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

        private static Mesh CreateQuad(float z)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[4];
            // Need to offset in z to properly order layers
            vertices[0] = new Vector3(0, 0, 10 - z);
            vertices[1] = new Vector3(1, 0, 10 - z);
            vertices[2] = new Vector3(0, 1, 10 - z);
            vertices[3] = new Vector3(1, 1, 10 - z);
            mesh.vertices = vertices;

            int[] tri = new int[6];
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;
            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;
            mesh.triangles = tri;

            Vector3[] normals = new Vector3[4];
            normals[0] = -Vector3.forward;
            normals[1] = -Vector3.forward;
            normals[2] = -Vector3.forward;
            normals[3] = -Vector3.forward;
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4];
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);
            mesh.uv = uv;

            return mesh;
        }
    }
}