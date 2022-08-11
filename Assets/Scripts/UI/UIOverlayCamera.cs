using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIOverlayCamera : MonoBehaviour
    {
        internal void SetTargetRawImage(ref RawImage rawImage)
        {
            var texture = new RenderTexture(80, 300, 16, RenderTextureFormat.ARGB32);
            texture.Create();

            var camera = GetComponent<Camera>();
            camera.targetTexture = texture;

            rawImage.texture = texture;
            var color = rawImage.color;
            color.a = 1.0f;
            rawImage.color = color;
        }
    }
}