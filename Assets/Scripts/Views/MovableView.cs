using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class MovableView : NetworkBehaviour
    {
        [Networked]
        public Vector3 speed { get; set; }

        public override void FixedUpdateNetwork()
        {
            if (Runner.Stage == SimulationStages.Forward)
                transform.position += speed * Runner.DeltaTime;
        }
    }
}
