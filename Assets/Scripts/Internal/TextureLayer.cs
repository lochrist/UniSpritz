using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace UniMini
{
    class TextureLayer : Layer
    {
        Texture2D m_Texture;
        NativeArray<Color32> m_Buffer;
        SpriteSheet m_Sheet;
        Material m_Material;
        int m_LayerIndex;
        Mesh m_Mesh;
        NativeArray<Color32> m_ClearBuffer;
        SpritzGame m_Game;
        Color m_ClearColor;

        public TextureLayer(SpritzGame game, SpriteSheet sheet, int layerIndex)
        {
            m_Game = game;
            m_Texture = new Texture2D(game.resolution.x, game.resolution.y, TextureFormat.RGBA32, false);
            m_Texture.filterMode = FilterMode.Point;
            m_Buffer = m_Texture.GetRawTextureData<Color32>();
            m_Sheet = sheet;

            var shader = Shader.Find("Custom/SpritzTexture");
            m_Material = new Material(shader);
            m_Material.mainTexture = m_Texture;
            m_LayerIndex = layerIndex;
            m_Mesh = Utils.CreateQuad((float)m_LayerIndex);

            m_ClearColor = new Color32(0, 0, 0, 255);
            m_ClearBuffer = new NativeArray<Color32>(game.resolution.x * game.resolution.y, Allocator.Persistent);
            for (var i = 0; i < m_ClearBuffer.Length; ++i)
                m_ClearBuffer[i] = m_ClearColor;

            SetupMeshRenderer();
        }

        public void CleanUp()
        {

        }

        public void Clear(Color c)
        {
            if (m_ClearColor != c)
            {
                m_ClearColor = c;
                for (var i = 0; i < m_ClearBuffer.Length; ++i)
                    m_ClearBuffer[i] = m_ClearColor;
            }

            m_ClearBuffer.CopyTo(m_Buffer);
        }

        public void DrawPixel(int x, int y, Color c)
        {
            m_Buffer[y * m_Texture.width + x] = c;
        }

        public void DrawSprite(SpriteId id, int x, int y)
        {
            var s = m_Sheet.GetSpriteById(id);
            // TODO: this is not optimized at all:

            // Note: sprite (0,0) is lower left.
            // Note: layer (0,0) is top left
            var layerX = x;
            for (var spriteX = (int)s.rect.x; spriteX < s.rect.xMax; ++spriteX, layerX++)
            {
                var layerY = y;
                for (var spriteY = (int)s.rect.yMax - 1; spriteY >= s.rect.yMin; --spriteY, layerY++)
                {
                    DrawPixel(layerX, layerY, m_Sheet.texture.GetPixel(spriteX, spriteY));
                }
            }
        }

        public void DrawText(string text, int x, int y, Color color)
        {
            var xOrig = x;
            foreach (char l in text)
            {
                if (l == '\n')
                {
                    y += 6;
                    x = xOrig;
                    continue;
                }

                if (DefaultFont.glyphs.ContainsKey(l))
                {
                    var glyph = DefaultFont.glyphs[l];
                    for (var i = 0; i < glyph.GetLength(0); i++)
                    {
                        for (var j = 0; j < glyph.GetLength(1); j++)
                        {
                            if (glyph[i, j] == 1)
                            {
                                DrawPixel(x + j, y + i, color);
                            }
                        }
                    }

                    x += glyph.GetLength(1) + 1;
                }
            }
        }

        public SpriteId[] GetSprites()
        {
            return m_Sheet.spriteDescriptors.Select(sd => sd.id).ToArray();
        }

        public void PreRender()
        {
            
        }

        public void Render()
        {
            // Could render similar to FuPixel using a mesh renderer + GameObject per layer
            // Could render using DrawMesh (similar to ComputeLayer)

            m_Texture.Apply();
        }

        private void SetupMeshRenderer()
        {
            MeshFilter meshFilter = m_Game.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = m_Game.gameObject.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = m_Game.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = m_Game.gameObject.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = m_Mesh;

            if (meshRenderer.sharedMaterials.Length == 0)
                meshRenderer.sharedMaterials = new Material[1];

            if (meshRenderer.sharedMaterials[0] == null)
            {
                Material[] materials = meshRenderer.sharedMaterials;
                materials[0] = m_Material;
                meshRenderer.sharedMaterials = materials;
            }

            m_Mesh.bounds = new Bounds(Vector3.zero, new Vector3(100000f, 100000f, 100000f));
            meshRenderer.sharedMaterials[0].SetTexture("_MainTex", m_Texture);

            Clear(m_ClearColor);
            m_Texture.Apply();
        }
    }

}