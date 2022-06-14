using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace UniMini
{
    // TO CHECK: should TextureLayer keeps color buffer and merge/blend with other layers (similar to UniPix) to generate a single texture?
    class TextureLayer : Layer
    {
        Texture2D m_Texture;
        NativeArray<Color32> m_Buffer;
        int m_BufferWidth;
        int m_BufferHeight;
        Color32[] m_SpriteSheetPixels;
        int m_SpriteSheetWidth;
        SpriteSheet m_Sheet;
        Material m_Material;
        int m_LayerIndex;
        Mesh m_Mesh;
        NativeArray<Color32> m_ClearBuffer;
        SpritzGame m_Game;
        Color32 m_ClearColor;

        public TextureLayer(SpritzGame game, SpriteSheet sheet, int layerIndex)
        {
            m_Game = game;
            m_Texture = new Texture2D(game.resolution.x, game.resolution.y, TextureFormat.RGBA32, false);
            m_Texture.filterMode = FilterMode.Point;
            m_Buffer = m_Texture.GetRawTextureData<Color32>();
            m_Sheet = sheet;
            m_BufferWidth = game.resolution.x;
            m_BufferHeight = game.resolution.y;

            var shader = Shader.Find("Custom/SpritzTexture");
            m_Material = new Material(shader);
            m_Material.mainTexture = m_Texture;
            m_LayerIndex = layerIndex;
            m_Mesh = Utils.CreateQuad((float)m_LayerIndex);

            m_ClearColor = new Color32(0, 0, 0, 255);
            m_ClearBuffer = new NativeArray<Color32>(game.resolution.x * game.resolution.y, Allocator.Persistent);
            for (var i = 0; i < m_ClearBuffer.Length; ++i)
                m_ClearBuffer[i] = m_ClearColor;

            if (sheet != null && sheet.texture != null)
            {
                m_SpriteSheetPixels = sheet.texture.GetPixels32();
                m_SpriteSheetWidth = sheet.texture.width;
            }
            else
            {
                m_SpriteSheetPixels = new Color32[0];
                m_SpriteSheetWidth = 0;
            }

            SetupMeshRenderer();
        }

        public void CleanUp()
        {

        }

        public void Clear(Color32 c)
        {
            if (!m_ClearColor.Equals(c))
            {
                m_ClearColor = c;
                for (var i = 0; i < m_ClearBuffer.Length; ++i)
                    m_ClearBuffer[i] = m_ClearColor;
            }

            m_ClearBuffer.CopyTo(m_Buffer);
        }

        public Color32 GetPixel(int x, int y)
        {
            return m_Buffer[y * m_BufferWidth + x];
        }

        public void DrawPixel(int x, int y, Color32 c)
        {
            if (x < 0 || x >= m_BufferWidth || y < 0 || y >= m_BufferHeight)
                return;
            var index = y * m_BufferWidth + x;
            if (index < 0 || index >= m_Buffer.Length)
                return;

            // TODO: should we assign color directly or should we multiply?
            m_Buffer[index] = c;
        }

        public void DrawPixels(int x, int y, int width, int height, Color32 c)
        {
            if (x + width >= m_BufferWidth)
                width = m_BufferWidth - x;
            if (y + height >= m_BufferHeight)
                height = m_BufferHeight - y;

            if (width <= 0 || height <= 0)
                return;

            var endY = y + height;
            for (; y < endY; ++y)
            {
                var dstIndex = y * m_Texture.width + x;
                for (var i = 0; i < width; ++i)
                    m_Buffer[dstIndex++] = c;
            }
        }

        public void DrawPixels(int x, int y, int width, int height, Color32[] colors)
        {
            if (width * height > colors.Length || 
                x >= m_BufferWidth ||
                y>= m_BufferHeight)
                return;
            var srcWidth = width;
            var srcXOffset = 0;
            if (x < 0)
            {
                srcXOffset = -x;
                width += x;
                x = 0;
            }

            var srcYOffset = 0;
            if (y < 0)
            {
                srcYOffset = -y;
                height += y;
                y = 0;
            }

            if (x + width >= m_BufferWidth)
                width = m_BufferWidth - x;
            if (y + height >= m_BufferHeight)
                height = m_BufferHeight - y;

            if (width <= 0 || height <= 0)
                return;

            var srcIndex = srcYOffset * srcWidth;
            var endY = y + height;
            for (; y < endY; ++y)
            {
                var dstIndex = y * m_Texture.width + x;
                NativeArray<Color32>.Copy(colors, srcIndex + srcXOffset, m_Buffer, dstIndex, width);
                srcIndex += srcWidth;
            }
        }

        public void DrawSprite(SpriteId id, int x, int y)
        {
            var s = m_Sheet.GetSpriteById(id);
            if (!s.isValid)
                return;
            // TODO: this is not optimized at all. We should copy multiple pixels at once.

            // Note: sprite (0,0) is lower left.
            // Note: layer (0,0) is top left
            var layerX = x;
            for (var spriteX = (int)s.rect.x; spriteX < s.rect.xMax; ++spriteX, layerX++)
            {
                var layerY = y;
                for (var spriteY = (int)s.rect.yMax - 1; spriteY >= s.rect.yMin; --spriteY, layerY++)
                {
                    var pixelIndex = spriteY * m_SpriteSheetWidth + spriteX;
                    var pix = m_SpriteSheetPixels[pixelIndex];
                    // TO CHECK: Do not copy transparent pixel: should we do alpha blending?
                    if (pix.a > 0)
                        DrawPixel(layerX, layerY, pix);
                }
            }
        }

        public void DrawSprite(SpriteId id, int x, int y, bool flipX, bool flipY, float angle)
        {
            DrawSpriteRotateWithFlip(id, x, y, flipX, flipY, angle);
            // FastDrawSpriteRotate(id, x, y, angle);
        }

        public void DrawSpriteRotateWithFlip(SpriteId id, int x, int y, bool flipX, bool flipY, float angle)
        {
            var s = m_Sheet.GetSpriteById(id);
            if (!s.isValid)
                return;

            var a = angle * Mathf.PI / 180f;
            var cosA = Mathf.Cos(a);
            var sinA = Mathf.Sin(a);
            var srcX = s.rect.x;
            var srcY = s.rect.y;
            var srcWidth = s.rect.width;
            var srcHeight = s.rect.height;
            var pivotX = srcWidth / 2f;
            var pivotY = srcHeight / 2f;
            var maxDx = pivotX - 0.5f;
            var maxDy = pivotY - 0.5f;
            var maxSqrDist = maxDx * maxDx + maxDy * maxDy;
            var maxDistMinusHalf = Mathf.Ceil(Mathf.Sqrt(maxSqrDist)) - 0.5f;
            for (var dx = -maxDistMinusHalf; dx < maxDistMinusHalf; ++dx)
            {
                for (var dy = -maxDistMinusHalf; dy < maxDistMinusHalf; ++dy)
                {
                    if (dx * dx + dy * dy < maxSqrDist)
                    {
                        var signX = flipX ? -1 : 1;
                        var signY = flipY ? -1 : 1;
                        var rotatedDx = signX * (cosA * dx + sinA * dy);
                        var rotatedDy = signY * (-sinA * dx + cosA * dy);
                        var xx = pivotX + rotatedDx;
                        var yy = pivotY + rotatedDy;
                        if (xx >= 0 && xx < srcWidth && yy >= 0 && yy < srcHeight)
                        {
                            var srcPixel = m_Sheet.texture.GetPixel((int)(srcX + xx), (int)(srcY + (srcHeight - 1 - yy)));
                            if (srcPixel.a > 0)
                            {
                                DrawPixel((int)(x + dx), (int)(y + dy), srcPixel);
                            }
                        }
                    }
                }
            }
        }

        public void FastDrawSpriteRotate(SpriteId id, int x, int y, float angle)
        {
            var s = m_Sheet.GetSpriteById(id);
            if (!s.isValid)
                return;

            var a = angle * Mathf.PI / 180f;
            var cosA = Mathf.Cos(a);
            var sinA = Mathf.Sin(a);
            var ddx0 = cosA;
            var ddy0 = sinA;
            var srcStartX = s.rect.x;
            var srcStartY = s.rect.y;
            var srcWidth = s.rect.width;
            var srcHeight = s.rect.height;
            var halfSrcWidth = srcWidth / 2;
            var halfSrcHeight = srcHeight / 2;
            var pivotX = halfSrcWidth - 0.5f;
            var pivotY = halfSrcHeight - 0.5f;
            var dx0 = (sinA * pivotX) - (cosA * pivotX) + halfSrcWidth;
            var dy0 = -(cosA * pivotY) - (sinA * pivotY) + halfSrcHeight;
            for (var ix = 0; ix < srcWidth; ++ix)
            {
                var srcOffsetX = dx0;
                var srcOffsetY = dy0;
                for (var iy = 0; iy < srcHeight; ++iy)
                {
                    var srcX = srcStartX + srcOffsetX;
                    var srcY = srcStartY + (srcHeight - 1 - srcOffsetY);
                    if (srcX >= 0 && srcOffsetX < srcWidth && srcY >= 0 && srcOffsetY < srcHeight)
                    {
                        var srcPixel = m_Sheet.texture.GetPixel((int)srcX, (int)srcY);
                        DrawPixel(x + ix, y + iy, srcPixel);
                    }
                    srcOffsetX -= ddy0;
                    srcOffsetY += ddx0;
                }
                dx0 += ddx0;
                dy0 += ddy0;
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
            var layerName = $"Layer_{m_LayerIndex}";
            GameObject layerObj = null;
            var layerTransform = m_Game.gameObject.transform.Find(layerName);
            if (layerTransform == null)
            {
                layerObj = new GameObject();
                layerObj.name = layerName;
                layerObj.transform.SetParent(m_Game.gameObject.transform);
            }
            else
            {
                layerObj = layerTransform.gameObject;
            }
            var newPos = layerObj.transform.localPosition;
            newPos.z -= m_LayerIndex * .25f;
            layerObj.transform.localPosition = newPos;

            var meshFilter = layerObj.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = layerObj.AddComponent<MeshFilter>();

            var meshRenderer = layerObj.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = layerObj.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = m_Mesh;

            if (meshRenderer.sharedMaterials.Length == 0)
                meshRenderer.sharedMaterials = new Material[1];

            if (meshRenderer.sharedMaterials[0] == null)
            {
                var materials = meshRenderer.sharedMaterials;
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