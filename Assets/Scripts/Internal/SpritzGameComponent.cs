using UnityEngine;

namespace UniMini
{
    public class SpritzGameComponent : MonoBehaviour
    {
        private void FixedUpdate()
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
    }
}