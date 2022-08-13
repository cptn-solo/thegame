using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDInventoryCanvasView : MonoBehaviour
    {
        private Canvas canvas;

        // Start is called before the first frame update
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }
        void Start()
        {
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;

        }
    }
}