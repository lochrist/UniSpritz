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

        #region Private
        private void OnEnable()
        {
            Spritz.Initialize(this.gameObject, this);
        }

        private void FixedUpdate()
        {
            Spritz.StartFrame();
            Spritz.Update();
            // TODO: probably not the right place to do so... should be in the camera OnPostRender.
            Spritz.Render();
            Spritz.RenderLayers();
            Spritz.EndFrame();
        }

        private void OnPreRender()
        {
            // Spritz.Render();
        }

        private void OnPostRender()
        {
            // Spritz.RenderLayers();
        }

        private void OnDisable()
        {
            Spritz.CleanUp();
        }
        #endregion

        public abstract void InitializeSpritz();
        public abstract void UpdateSpritz();
        public abstract void DrawSpritz();
    }
}