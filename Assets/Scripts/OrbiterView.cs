using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbiterView : NetworkBehaviour
{
    [SerializeField] private float satMoveSpeed = 10;
    
    [HideInInspector] public Transform AnchorTransform;

    public override void FixedUpdateNetwork()
    {
        Debug.LogFormat("FUN satellite: {0}", AnchorTransform);

        if (!AnchorTransform)
            return;

        var distance = AnchorTransform.position - transform.position;
        transform.position += distance * Runner.DeltaTime * satMoveSpeed;

    }

    private void Update()
    {
        if (!AnchorTransform)
            return;

        var distance = (AnchorTransform.position - transform.position) * Time.deltaTime * satMoveSpeed;
        transform.position += distance;

    }


}
