using UnityEngine;

namespace UniMini
{
    public class SpritzGameComponent : MonoBehaviour
    {
        void Update()
        {
            Spritz.Update();
        }

        private void OnPreRender()
        {
            Spritz.Render();
        }

        void OnPostRender()
        {
            Spritz.RenderLayers();
        }
    }
}