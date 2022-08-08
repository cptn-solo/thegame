using UnityEngine;

namespace Assets.Scripts.Views
{
    public class ToggleAnimationOnCollider : MonoBehaviour
    {
        [SerializeField] private string clipName = "Take 001";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            TryAnimationStartPlayback();
        }

        private void TryAnimationStartPlayback()
        {
            if (animator)
            {
                animator.Play(clipName, -1, 0.0f);
            }
        }
    }
}