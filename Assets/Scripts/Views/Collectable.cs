using Assets.Scripts.Data;
using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class Collectable : NetworkBehaviour
    {
        private const string animator_collected_bool = "collected_bool";
        [SerializeField] private CollectableType collectableType = CollectableType.Diamond;
        [SerializeField] private Animator animator = null;

        [Networked] public NetworkBool collected { get; set; }
        
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Collectable entered");
            
            if (other.gameObject.transform.parent.TryGetComponent<Collector>(out var collector))
            {
                collected = true;
                Debug.Log($"Collector present");
                collector.Collect(collectableType, 1);

                if (animator != null)
                {
                    animator.SetBool(animator_collected_bool, true);
                    //animator.Play(despawnAnimationClipName, -1, 0.0f);
                }
            }
        }
    }
}
