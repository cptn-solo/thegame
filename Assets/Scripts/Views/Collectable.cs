using Assets.Scripts.Data;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class Collectable : NetworkBehaviour
    {
        private const string animator_collected_bool = "collected_bool";

        [SerializeField] private Animator animator = null;
        
        private CollectableType collectableType;

        [Networked(OnChanged = nameof(OnCollectionStateChange))]
        public NetworkDictionary<NetworkId, CollectionState> Collected => default;

        private static void OnCollectionStateChange(Changed<Collectable> changed)
        {
            var hasValue = changed.Behaviour.Collected.TryGet(changed.Behaviour.Object.Id, out var currentState);

            changed.LoadOld();
            changed.Behaviour.Collected.TryGet(changed.Behaviour.Object.Id, out var prevState);

            if (hasValue && currentState != prevState)
                changed.Behaviour.ApplyCollectionState(currentState);
        }

        private void Awake()
        {
            var infoComponents = transform.parent.GetComponentsInChildren<CollectableInfo>();
            if (infoComponents.Length > 0)
                collectableType = infoComponents[0].CollectableType;
        }

        public void ApplyCollectionState(CollectionState currentState)
        {
            switch (currentState)
            {
                case CollectionState.Collected:
                    {
                        transform.parent.gameObject.SetActive(false);
                        break;

                    }
                case CollectionState.Collecting:
                    {
                        if (!animator)
                            break;

                        animator.SetBool(animator_collected_bool, true);
                        break;
                    }
            }

        }

        internal void SetCollectedState(CollectionState state) =>
            Collected.Set(Object.Id, state);

        private void OnTriggerEnter(Collider other)
        {
            if (!Collected.TryGet(Object.Id, out _) &&
                other.gameObject.transform.parent.TryGetComponent<Collector>(out var collector))
            {
                SetCollectedState(CollectionState.Collecting);
                
                if (collector.Object.InputAuthority == Runner.LocalPlayer)
                    collector.Collect(collectableType, 1);
            }
        }
    }
}
