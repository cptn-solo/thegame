using Assets.Scripts.Data;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private CollectableType collectableType;
        [SerializeField] private string despawnAnimationClipName;

        [Networked] public NetworkBool collected { get; set; }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Collector>(out var collector))
            {
                collected = true;

                collector.Collect(collectableType, 1);

                if (TryGetComponent<Animator>(out var animator))
                {
                    animator.Play(despawnAnimationClipName, -1, 0.0f);
                }
            }
        }
    }
}
