using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIOverlayCamera : MonoBehaviour
    {
        private RenderTexture texture;

        internal void SetTargetRawImage(ref RawImage rawImage)
        {
            texture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            texture.Create();

            var camera = GetComponent<Camera>();
            camera.targetTexture = texture;

            rawImage.texture = texture;
        }
    }
}