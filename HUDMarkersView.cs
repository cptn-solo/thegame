using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public struct MarkerInfo
{
    public string Label;
    public Color LabelColor;
    public string Info;

    public static MarkerInfo Get(string label, Color color, string info)
    {
        MarkerInfo marker = default;
        marker.Label = label;
        marker.LabelColor = color;
        marker.Info = info;

        return marker;
    }

    public void SetInfo(string label, Color color, string info)
    {
        Label = label;
        LabelColor = color;
        Info = info;
    }
}
public class HUDMarkersView : MonoBehaviour
{
    [SerializeField] private GameObject markerPrefab;

    private Dictionary<Transform, HUDMarkerView> markers;
    
    private RectTransform rectTransform;

    internal void AddPlayer(Transform transform)
    {
        var marker = GameObject.Instantiate(markerPrefab).GetComponent<HUDMarkerView>();
        marker.Attach(rectTransform, transform);
        markers.Add(transform, marker);
    }

    internal void RemovePlayer(Transform transform)
    {
        if (markers.TryGetValue(transform, out var marker) && !marker.IsDestroyed())
        {
            marker.Detach();
            GameObject.Destroy(marker.gameObject);
        }
    }

    internal void UpdatePlayer(Transform transform, Assets.Scripts.Services.App.PlayerInfo playerInfo)
    {
        if (markers.TryGetValue(transform, out var marker) && !marker.IsDestroyed())
        {
            MarkerInfo markerInfo = default;

            markerInfo.Label = playerInfo.NickName;
            markerInfo.LabelColor = playerInfo.BodyTintColor;
            markerInfo.Info = $"{playerInfo.Score}";

            marker.Info = markerInfo;
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
