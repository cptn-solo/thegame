using System;
using TMPro;
using UnityEngine;

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

    public void Attach(RectTransform parent, Transform worldTarget)
    {
        rectTransform.SetParent(parent);
        this.worldTarget = worldTarget;
    }

    public void Detach()
    {
        rectTransform.parent = null;
        this.worldTarget = null;
    }

    private void LateUpdate()
    {
        if (Camera.main != null && worldTarget != null)
        {
            var pos = worldTarget.position;

            var distance = Vector3.Distance(Camera.main.transform.position, worldTarget.position);
            if (distance == 0)
                return;
            float factor = 1;

            factor = 1 - Mathf.Atan(distance * .1f) * .5f;

            //if (distance > 10) factor = .8f;
            //if (distance > 20) factor = .7f;
            //if (distance > 30) factor = .6f;
            //if (distance > 50) factor = .5f;
            //if (distance > 100) factor = .4f;
            //if (distance > 300) factor = .2f;


            rectTransform.position = Camera.main.WorldToScreenPoint(pos);
            rectTransform.position += Vector3.up * factor * 140;
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
