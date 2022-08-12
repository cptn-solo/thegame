using Example;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class PlayerMove : NetworkBehaviour
    {
        [SerializeField] private Animator animator;

        public override void FixedUpdateNetwork()
        {
            if (Runner.TryGetInputForPlayer(Object.InputAuthority, out GameplayInput input))
                animator.SetBool("Forward", input.MoveDirection.magnitude > 0);
        }
    }
}