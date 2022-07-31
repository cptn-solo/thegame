using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSatelliteVisual : NetworkBehaviour
{

    [SerializeField] private Transform anchorPosition;
    [SerializeField] private GameObject satellitePrefab;

    public override void Spawned()
    {
        Runner.Spawn(satellitePrefab, anchorPosition.position, Quaternion.identity, Runner.LocalPlayer, InitSatellite);
    }

    private void InitSatellite(NetworkRunner runner, NetworkObject obj)
    {
        var orbiterView = obj.GetComponent<OrbiterView>();
        orbiterView.AnchorTransform = anchorPosition;
    }
}
