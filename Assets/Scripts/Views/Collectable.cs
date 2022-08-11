using Assets.Scripts.Data;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class Collectable : NetworkBehaviour
    {
        private const string animator_collected_bool = "collected_bool";
        private const string anim_collection_state_name = "Collection";

        [SerializeField] private Animator animator = null;
        
        private CollectableType collectableType;

        [Networked] public NetworkBool collected { get; set; }

        private void Awake()
        {
            var infoComponents = transform.parent.GetComponentsInChildren<CollectableInfo>();
            if (infoComponents.Length > 0)
                collectableType = infoComponents[0].CollectableType;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Collectable entered");
            
            if (!collected && other.gameObject.transform.parent.TryGetComponent<Collector>(out var collector))
            {
                collected = true;
                Debug.Log($"Collector present");
                collector.Collect(collectableType, 1);

                if (animator != null)
                {
                    animator.SetBool(animator_collected_bool, true);
                    animator.Play(anim_collection_state_name, -1, 0.0f);
                }
            }
        }
    }
}
