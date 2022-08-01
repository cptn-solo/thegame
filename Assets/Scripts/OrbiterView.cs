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
        nt.InterpolationDataSource = InterpolationDataSources.Snapshots;
        var no = Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<PlayerSatelliteVisual>(anchor);
        if (no)
            transform.parent = no.AnchorPosition;
    }
}
