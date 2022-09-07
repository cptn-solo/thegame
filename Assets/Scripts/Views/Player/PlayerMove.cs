using Example;
using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class PlayerMove : NetworkBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpeedEnhancer speedEnhancer;
        private AudioSource[] audioSource;
        private KCC kcc;

        private void Awake()
        {
            audioSource = GetComponents<AudioSource>();
            kcc = GetComponentInParent<KCC>();
        }

        private void Start()
        {
            audioSource[1].Play();
            audioSource[1].volume = .5f;
        }

        public override void FixedUpdateNetwork()
        {
            bool forward = false;
            float speed = 0.0f;
            if (Runner.Stage == SimulationStages.Forward &&
                Runner.TryGetInputForPlayer(Object.InputAuthority, out GameplayInput input))
            {
                forward = input.MoveDirection.magnitude > 0;
                speed = speedEnhancer.SpeedEnhancerValue;
            }
            animator.SetBool("Forward", forward);
            animator.SetFloat("MoveSpeed", speed);
                        
            if (forward && kcc.FixedData.IsGrounded)
            {
                if (audioSource[0].isPlaying)
                    audioSource[0].UnPause();
                else
                    audioSource[0].Play();
                audioSource[0].volume = 1.0f;
                audioSource[0].pitch = speed;
            }
            else
            {
                audioSource[0].volume = .01f;
                //audioSource.Pause();
            }

            if (forward)
            {
                if (audioSource[1].isPlaying)
                    audioSource[1].UnPause();
                else
                    audioSource[1].Play();
                audioSource[1].volume = 1.0f;
                audioSource[1].pitch = speed;
            }
            else
            {
                audioSource[1].volume = .5f;
                //audioSource.Pause();
            }

        }
    }
}