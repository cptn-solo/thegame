using Fusion;
using UnityEngine;

public class PlayerSatelliteVisual : NetworkBehaviour
{

    [SerializeField] private Transform anchorPosition;
    [SerializeField] private GameObject satellitePrefab;

    public Transform AnchorPosition => anchorPosition;

    public override void Spawned()
    {
        Runner.Spawn(satellitePrefab, anchorPosition.position, Quaternion.identity, Object.InputAuthority, InitSatellite);
    }

    private void InitSatellite(NetworkRunner runner, NetworkObject obj)
    {
        var orb = obj.GetComponent<OrbiterView>();
        var noGuid = Object.Id;
        orb.anchor = noGuid;
    }

}
