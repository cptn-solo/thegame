using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIItemDisplayPanel : MonoBehaviour
    {
        [SerializeField] private UIOverlayCamera uiOverlayCamera;

        void Start()
        {
            var rawImage = GetComponent<RawImage>();
            uiOverlayCamera.SetTargetRawImage(ref rawImage);
        }
    }
}