#define DEBUG_SPRITZ
using UnityEngine;

namespace UniMini
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public abstract class SpritzGame : MonoBehaviour
    {
        public int pixelPerUnit = 32;
        public Vector2Int resolution = new Vector2Int(128, 128);
        public LayerType layerType = LayerType.Texture;
        public int numberSfxChannels = 4;
        public int fps = 30;
        #if DEBUG_SPRITZ
        public string debugPerformanceTitle;
        #endif

#region Private
        private void OnEnable()
        {
            Spritz.Initialize(gameObject, this);
        }

        private void FixedUpdate()
        {
#if DEBUG_SPRITZ
            Spritz.StartFrame();

            if (!string.IsNullOrEmpty(debugPerformanceTitle))
            {
                using (new Utils.Timer($"{debugPerformanceTitle} Update"))
                    Spritz.Update();
                // TODO: probably not the right place to do so... should be in the camera OnPostRender.

                using (new Utils.Timer($"{debugPerformanceTitle} Render"))
                    Spritz.Render();

                using (new Utils.Timer($"{debugPerformanceTitle} RenderLayers"))
                    Spritz.RenderLayers();
            }
            else
            {
                Spritz.Update();
                Spritz.Render();
                Spritz.RenderLayers();
            }

            Spritz.EndFrame();
#else
            Spritz.StartFrame();
            Spritz.Update();
            // TODO: probably not the right place to do so... should be in the camera OnPostRender.
            Spritz.Render();
            Spritz.RenderLayers();
            Spritz.EndFrame();
#endif
        }

        private void OnDisable()
        {
            Spritz.CleanUp();
        }
#endregion

        public abstract void InitializeSpritz();
        public virtual void StartFrame()
        {
        }
        public abstract void UpdateSpritz();
        public abstract void DrawSpritz();
        public virtual void EndFrame()
        {
        }
    }
}