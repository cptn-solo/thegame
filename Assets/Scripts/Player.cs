using Cinemachine;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cameraPrefab;
    private NetworkCharacterControllerPrototype _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void Spawned()
    {
        base.Spawned();
        var comp = GetBehaviour<NetworkObject>();
        if (comp.HasInputAuthority)
        {
            var cam = Instantiate(cameraPrefab);
            cam.LookAt = comp.transform;
            cam.Follow = comp.transform;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
        }
    }
}
