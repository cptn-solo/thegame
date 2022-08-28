using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDMarkersView : MonoBehaviour
    {
        [SerializeField] private GameObject markerPrefab;
        private readonly Dictionary<Transform, HUDMarkerView> markers = new();

        private RectTransform rectTransform;

        internal HUDMarkerView AddPlayer(Transform transform)
        {
            var marker = Instantiate(markerPrefab).GetComponent<HUDMarkerView>();
            marker.Attach(rectTransform, transform);
            markers.Add(transform, marker);
            return marker;
        }

        internal void RemovePlayer(Transform transform)
        {
            if (markers.TryGetValue(transform, out var marker) && !marker.gameObject.IsDestroyed())
            {
                marker.Detach();
                Destroy(marker.gameObject);
            }
        }

        internal void UpdatePlayer(Transform transform, Services.App.PlayerInfo playerInfo)
        {
            if (markers.TryGetValue(transform, out var marker) && !marker.gameObject.IsDestroyed())
                marker.SetInfo(playerInfo.NickName, playerInfo.BodyTintColor, $"{playerInfo.Score}");
        }

        private void Awake() => rectTransform = GetComponent<RectTransform>();
    }
}