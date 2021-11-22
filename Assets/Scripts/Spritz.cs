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

    public struct AudioClipId
    {
        public int value;

        public AudioClipId(string name)
        {
            value = name.GetHashCode();
        }

        public AudioClipId(int id)
        {
            value = id;
        }

        public bool isValid => value != 0;

        public bool Equals(AudioClipId other)
        {
            return value == other.value;
        }

        public override bool Equals(object other)
        {
            return value == ((AudioClipId)other).value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static bool operator ==(AudioClipId a, AudioClipId b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(AudioClipId a, AudioClipId b)
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
        static GameObject m_Root;
        static Camera m_Camera;
        static SpritzGame m_Game;
        static List<Layer> m_Layers;
        static Layer currentLayer => m_Layers[m_CurrentLayerId];
        static int m_CurrentLayerId;

        static int m_CurrentSoundbankId;
        static List<Soundbank> m_Soundbanks;
        static Soundbank currentSoundBank => m_Soundbanks[m_CurrentSoundbankId];

        static List<AudioSource> m_SfxChannels;
        static AudioSource m_MusicChannel;

        internal static float zoom;
        internal static float pixelPerUnit => m_Game.pixelPerUnit;
        internal static float unitsPerPixel;

        public static void Initialize(GameObject root, SpritzGame game)
        {
            m_Root = root;
            root.transform.position = new Vector3(0f, 0f, -10f);
            root.transform.rotation = Quaternion.identity;

            m_Layers = new List<Layer>(4);
            
            m_Game = game;
            CreateCamera();
            SetupCamera();

            SetupAudio();
            game.enabled = true;
            game.InitializeSpritz();
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
        public static Vector2Int camera;

        public static void DrawSprite(SpriteId id, int x, int y)
        {
            CameraClip(ref x, ref y);
            currentLayer.DrawSprite(id, x - camera.x, y - camera.y);
        }

        public static void Circle(int x, int y, int radius, Color color, bool fill)
        {
            CameraClip(ref x, ref y);
            if (fill)
                PixelDrawing.DrawFilledCircle(currentLayer, x, y, radius, color);
            else
                PixelDrawing.DrawCircle(currentLayer, x, y, radius, color);
        }

        public static void Ellipse(int x0, int y0, int x1, int y1, Color color, bool fill)
        {
            CameraClip(ref x0, ref y0);
            CameraClip(ref x1, ref y1);
        }

        public static void Line(int x0, int y0, int x1, int y1, Color color)
        {
            CameraClip(ref x0, ref y0);
            CameraClip(ref x1, ref y1);
            PixelDrawing.DrawLine(currentLayer, new Vector2Int(x0, y0), new Vector2Int(x1, y1), color);
        }

        public static void Rectangle(int x0, int y0, int x1, int y1, Color color, bool fill)
        {
            CameraClip(ref x0, ref y0);
            CameraClip(ref x1, ref y1);
            if (fill)
                PixelDrawing.DrawFilledRectangle(currentLayer, new RectInt(x0, y0, x1, y1), color);
            else
                PixelDrawing.DrawRectangle(currentLayer, new RectInt(x0, y0, x1, y1), color);
        }

        public static RectInt Clip(int x0, int y0, int x1, int y1)
        {
            CameraClip(ref x0, ref y0);
            CameraClip(ref x1, ref y1);
            return new RectInt();
        }

        public static void Print(string text, int x, int y, Color color)
        {
            CameraClip(ref x, ref y);
            currentLayer.DrawText(text, x, y, color);
        }

        public static Vector2Int PrintSize(string text)
        {
            return new Vector2Int();
        }

        public static void DrawPixel(int x, int y, Color color)
        {
            CameraClip(ref x, ref y);
            currentLayer.DrawPixel(x, y, color);
        }

        public static void Clear(Color color)
        {
            currentLayer.Clear(color);
        }
        #endregion

        #region Sound
        public static int LoadSoundbankFolder(string folder)
        {
            return LoadSoundbank(Soundbank.LoadFromFolder(folder));
        }

        public static int LoadSoundbankAsset(string path)
        {
            return LoadSoundbank(Soundbank.LoadFromFile(path));
        }

        public static int LoadSoundbank(AudioClip[] clips)
        {
            return LoadSoundbank(new Soundbank() { clips = clips });
        }

        public static int currentSoundbankId
        {
            get => m_CurrentSoundbankId;
            set
            {
                m_CurrentSoundbankId = value;
                if (value >= m_Soundbanks.Count || value < 0)
                    m_CurrentSoundbankId = 0;
            }
        }

        public static void PlayMusic(AudioClipId id, float volume, bool loop)
        {
            m_MusicChannel.Stop();

            var clip = currentSoundBank.GetClip(id);
            if (clip == null)
                return;
            m_MusicChannel.clip = clip;
            m_MusicChannel.Play();
        }

        public static void StopMusic()
        {
            m_MusicChannel.Stop();
        }

        public static int PlaySound(AudioClipId id, float volume, bool loop)
        {
            var clip = currentSoundBank.GetClip(id);
            if (clip == null)
                return -1;

            var channel = GetFreeSfxChannel();
            if (channel != -1)
            {
                m_SfxChannels[channel].clip = clip;
                m_SfxChannels[channel].Play();
            }
            return channel;
        }

        public static void StopSound(int i)
        {
            if (i > 0 && i < m_SfxChannels.Count && m_SfxChannels[i].isPlaying)
            {
                m_SfxChannels[i].Stop();
            }
        }
        #endregion

        #region Internals
        private static void CameraClip(ref int x, ref int y)
        {
            x -= Spritz.camera.x;
            y -= Spritz.camera.y;
        }

        private static void CreateCamera()
        {
            var camera = m_Root.GetComponent<Camera>();
            if (camera == null)
            {
                m_Camera = m_Root.AddComponent<Camera>();
            }
            m_Camera = camera;
            Spritz.camera = new Vector2Int(0, 0);
        }

        private static void SetupCamera()
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

        private static int LoadSoundbank(Soundbank sb)
        {
            if (sb == null)
                return currentSoundbankId;

            m_Soundbanks.Add(sb);
            currentSoundbankId = m_Soundbanks.Count;
            return currentSoundbankId;
        }

        private static int CreateLayer(SpriteSheet spriteSheet)
        {
            var layer = (Layer)(m_Game.layerType == LayerType.Compute ? new ComputeLayer(m_Game, spriteSheet, m_Layers.Count) : new TextureLayer(m_Game, spriteSheet, m_Layers.Count));
            m_Layers.Add(layer);
            currentLayerId = m_Layers.Count - 1;
            return currentLayerId;
        }

        private static void SetupAudio()
        {
            var audioListener = m_Root.GetComponent<AudioListener>();
            if (audioListener == null)
            {
                m_Root.AddComponent<AudioListener>();
            }

            m_Soundbanks = new List<Soundbank>();

            m_MusicChannel = CreateAudioChannel("music_channel");
            m_SfxChannels = new List<AudioSource>(m_Game.numberSfxChannels);
            for(var i = 0; i < m_Game.numberSfxChannels; ++i)
            {
                m_SfxChannels.Add(CreateAudioChannel($"sfx_channel_{i}"));
            }
        }

        private static AudioSource CreateAudioChannel(string name)
        {
            var channelTransform = m_Game.transform.Find(name);
            if (channelTransform == null)
            {
                var obj = new GameObject();
                obj.name = name;
                obj.transform.SetParent(m_Game.transform);
                channelTransform = obj.transform;
            }
            var channel = channelTransform.gameObject.GetComponent<AudioSource>();
            if (channel == null)
            {
                channel = channelTransform.gameObject.AddComponent<AudioSource>();
            }
            return channel;
        }

        private static int GetFreeSfxChannel()
        {
            for(var i = 0; i < m_SfxChannels.Count; ++i)
            {
                if (m_SfxChannels[i].clip == null)
                    return i;
                if (!m_SfxChannels[i].isPlaying)
                    return i;
            }
            return -1;
        }

        #endregion
    }
}