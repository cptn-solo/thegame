using Fusion;
using UnityEngine;

public class OrbiterView : NetworkBehaviour
{
    private NetworkTransform nt;
    [Networked, HideInInspector] public NetworkId anchor { get; set; }


    private void Awake()
    {
        Debug.Log("ORBITER AWAKE");
        nt = GetComponent<NetworkTransform>();
    }

    public override void Spawned()
    {
        var no = Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<PlayerSatelliteVisual>(anchor);
        if (no)
            nt.transform.parent = no.AnchorPosition;
    }
}
