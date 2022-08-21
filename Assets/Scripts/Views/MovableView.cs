using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class MovableView : NetworkBehaviour
    {
        private Rigidbody rb;

        [Networked]
        public Vector3 speed { get; set; }

        [Networked]
        public float mass { get; set; } = .002f;

        [Networked]
        public NetworkBool useGravity { get; set; } = false;

        private void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody>();
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.Stage == SimulationStages.Forward && speed != default && rb != null)
            {
                rb.mass = mass;
                rb.useGravity = useGravity;
                rb.AddForce(speed, ForceMode.VelocityChange);
                speed = default;
            }
            //transform.position += speed * Runner.DeltaTime;
        }
    }
}
