using Fusion;
using System;
using UnityEngine;

public class PlayerSatelliteVisual : NetworkBehaviour
{

    [SerializeField] private NetworkTransformAnchor anchorPosition;
    [SerializeField] private GameObject satellitePrefab;

    public NetworkTransformAnchor AnchorPosition => anchorPosition;

    [Networked (OnChanged = nameof(AnchorSidesRefChanged)), HideInInspector] public NetworkId anchorRef { get; set; }
    [Networked (OnChanged = nameof(AnchorSidesRefChanged)), HideInInspector] public NetworkId satelliteRef { get; set; }

    protected static void AnchorSidesRefChanged(Changed<PlayerSatelliteVisual> changed)
    {
        var curAncorRef = changed.Behaviour.anchorRef;
        var curSatelliteRef = changed.Behaviour.satelliteRef;

        if (curAncorRef && curSatelliteRef)
            changed.Behaviour.AttachSatellite(curAncorRef, curSatelliteRef);
    }

    private void AttachSatellite(NetworkId curAncorRef, NetworkId curSatelliteRef)
    {
        Debug.LogFormat("AttachSatellite: {0} to: {1}", curSatelliteRef, curAncorRef);

        var playerSatAnchorNTA = Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<NetworkTransformAnchor>(curAncorRef);
        var orbiterNTA = Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<NetworkTransformAnchor>(curSatelliteRef);
        if (playerSatAnchorNTA && orbiterNTA)
        {
            Debug.LogFormat("AttachedSatellite: {0} to: {1}", curSatelliteRef, curAncorRef);
            orbiterNTA.transform.parent = playerSatAnchorNTA.transform;
            orbiterNTA.transform.SetPositionAndRotation(playerSatAnchorNTA.transform.position, playerSatAnchorNTA.transform.rotation);
        }
    }

    public override void Spawned()
    {
        Runner.Spawn(satellitePrefab, anchorPosition.transform.position, Quaternion.identity, Object.InputAuthority, InitSatellite);
    }

    private void InitSatellite(NetworkRunner runner, NetworkObject obj)
    {
        anchorRef = Object.Id;
        satelliteRef = obj.Id;
    }

}
