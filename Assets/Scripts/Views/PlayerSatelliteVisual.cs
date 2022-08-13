using Fusion;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerSatelliteVisual : NetworkBehaviour
    {

        [SerializeField] private NetworkTransformAnchor anchorPosition;
        [SerializeField] private GameObject satellitePrefab;

        public NetworkTransformAnchor AnchorPosition => anchorPosition;

        public override void Spawned()
        {
            Runner.Spawn(satellitePrefab, anchorPosition.transform.position, Quaternion.identity, Object.InputAuthority,
                (runner, obj) => obj.GetComponent<OrbiterView>().InitSatelliteForAnchorRef(Object.Id));
        }

    }
}