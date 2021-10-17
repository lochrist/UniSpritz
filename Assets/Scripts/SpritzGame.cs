using UnityEngine;

namespace UniMini
{
    public abstract class SpritzGame : MonoBehaviour
    {
        #region Private
        private void Awake()
        {
            Spritz.Initialize(this.gameObject, this);
        }

        private void Update()
        {
            Spritz.Update();
            // TODO: probably not the right place to do so... should be in the camera OnPostRender.
            Spritz.Render();
            Spritz.RenderLayers();
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