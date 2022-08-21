using Example;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class PlayerMove : NetworkBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpeedEnhancer speedEnhancer;

        public override void FixedUpdateNetwork()
        {
            if (Runner.Stage == SimulationStages.Forward &&
                Runner.TryGetInputForPlayer(Object.InputAuthority, out GameplayInput input))
            {
                animator.SetBool("Forward", input.MoveDirection.magnitude > 0);
                animator.SetFloat("MoveSpeed", speedEnhancer.SpeedEnhancerValue);
            }
        }
    }
}