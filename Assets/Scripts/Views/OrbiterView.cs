using Fusion;
using UnityEngine;

namespace Assets.Scripts
{
    public class OrbiterView : NetworkBehaviour
    {

        [SerializeField] private NetworkMecanimAnimator networkAnimator;

        private void Start() =>
            networkAnimator.Animator.SetBool("test_bool_param", false);
    }
}