using Fusion;
using UnityEngine;

public class OrbiterView : NetworkBehaviour
{
    [Networked(OnChanged = nameof(AnchorSidesRefChanged))]
    private NetworkId anchorRef { get; set; }
    
    [Networked(OnChanged = nameof(AnchorSidesRefChanged))]
    private NetworkId satelliteRef { get; set; }


    public void InitSatelliteForAnchorRef(NetworkId anchorRef)
    {
        this.anchorRef = anchorRef;
        satelliteRef = Object.Id;
    }

    protected static void AnchorSidesRefChanged(Changed<OrbiterView> changed)
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
            orbiterNTA.transform.SetParent(playerSatAnchorNTA.transform);
            orbiterNTA.transform.SetPositionAndRotation(playerSatAnchorNTA.transform.position, playerSatAnchorNTA.transform.rotation);
        }
    }
}
