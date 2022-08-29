using System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class HUDMarkerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI infoLabel;

        private RectTransform rectTransform;
        private Transform worldTarget;

        private MarkerInfo info = default;

        public MarkerInfo Info => info;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Attach(RectTransform parent, Transform worldTarget, float scale)
        {
            rectTransform.SetParent(parent);
            rectTransform.localScale *= scale;

            this.worldTarget = worldTarget;
        }

        public void Detach()
        {
            rectTransform.parent = null;
            worldTarget = null;
            this.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (Camera.main != null && worldTarget != null)
            {
                var pos = worldTarget.position;

                var distance = Vector3.Distance(Camera.main.transform.position, worldTarget.position);
                if (distance == 0)
                    return;

                var factor = 1 - Mathf.Atan(distance * .1f) * .5f;

                rectTransform.position = Camera.main.WorldToScreenPoint(pos);
                rectTransform.position += 140 * factor * Vector3.up;
                rectTransform.localScale = Vector3.one * factor;
            }

        }

        internal void SetInfo(string label, Color color, string info)
        {
            Info.SetInfo(label, color, info);

            titleLabel.text = label;
            titleLabel.color = color;
            infoLabel.text = info;
        }
    }
}