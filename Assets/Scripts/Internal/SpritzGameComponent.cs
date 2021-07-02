using UnityEngine;

namespace UniMini
{
    public class SpritzGameComponent : MonoBehaviour
    {
        private void Update()
        {
            Spritz.Update();
        }

        private void OnPreRender()
        {
            Spritz.Render();
        }

        private void OnPostRender()
        {
            Spritz.RenderLayers();
        }

        private void OnDisable()
        {
            Spritz.CleanUp();
        }
    }
}