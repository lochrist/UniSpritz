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

        public Color GetPixel(int x, int y)
        {
            return m_Buffer[y * m_Texture.width + x];
        }

        public void DrawPixel(int x, int y, Color c)
        {
            if (x < 0 || x >= m_Game.resolution.x || y < 0 || y >= m_Game.resolution.y)
                return;
            var index = y * m_Texture.width + x;
            if (index < 0 || index >= m_Buffer.Length)
                return;

            // TODO: should we assign color directly or should we multiply?
            m_Buffer[index] = c;
        }

        public void DrawPixels(int x, int y, int width, int height, Color[] c)
        {
            if (width * height > c.Length || 
                x >= m_Game.resolution.x ||
                y>= m_Game.resolution.y)
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

            if (x + width >= m_Game.resolution.x)
                width = m_Game.resolution.x - x;
            if (y + height >= m_Game.resolution.y)
                height = m_Game.resolution.y - y;

            if (width <= 0 || height <= 0)
                return;

            // TODO: conversion to Color32 is potentially costly here.
            var colors = c.Select(c => (Color32)c).ToArray();
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
                    DrawPixel(layerX, layerY, m_Sheet.texture.GetPixel(spriteX, spriteY));
                }
            }
        }

        public void DrawSprite(SpriteId id, int x, int y, float angle)
        {
            var s = m_Sheet.GetSpriteById(id);
            if (!s.isValid)
                return;
            var cosA = Mathf.Cos(angle);
            // var cosA = SpritzUtil.Cosp8(angle);
            var sinA = Mathf.Sin(angle);
            // var sinA = SpritzUtil.Sinp8(angle);
            var ddx0 = cosA;
            var ddy0 = sinA;
            var srcStartX = s.rect.x;
            var srcStartY = s.rect.y;
            var srcWidth = s.rect.width;
            var srcHeight = s.rect.height;
            var halfSrcWidth = srcWidth / 2;
            var halfSrcHeight = srcHeight / 2;
            var dx0 = (sinA * halfSrcWidth) + (cosA * halfSrcHeight) + halfSrcWidth;
            var dy0 = (-cosA * halfSrcHeight) - (sinA * halfSrcWidth) + halfSrcHeight;
            for(var ix = 0; ix < srcWidth; ++ix)
            {
                var srcOffsetX = dx0;
                var srcOffsetY = dy0;
                for (var iy = 0; iy < srcHeight; ++iy)
                {
                    var srcX = srcStartX + srcOffsetX;
                    var srcY = srcStartY + srcOffsetY;
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