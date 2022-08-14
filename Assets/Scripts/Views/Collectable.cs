using Assets.Scripts.Data;
using Fusion;
using System;
using UnityEngine;

namespace Assets.Scripts.Views
{

    public class Collectable : NetworkBehaviour
    {
        private const string animator_collected_bool = "collected_bool";

        [SerializeField] private Animator animator = null;

        public CollectableType CollectableType { get; private set; }

        [Networked(OnChanged = nameof(OnCollectionStateChange))]
        public NetworkDictionary<CollectionState, NetworkId> Collected => default;

        private static void OnCollectionStateChange(Changed<Collectable> changed)
        {
            var currState = changed.Behaviour.Collected;

            changed.LoadOld();
            var prevState = changed.Behaviour.Collected;

            if (!currState.Equals(prevState))
                changed.Behaviour.ApplyCollectionState(currState);
        }

        private void Awake()
        {
            var infoComponent = GetComponentInChildren<CollectableInfo>();
            if (infoComponent)
                CollectableType = infoComponent.CollectableType;
        }

        internal void SetCollectedState()
        {
            Collected.Set(CollectionState.Collected, Object.Id);
        }

        public void ApplyCollectionState(NetworkDictionary<CollectionState, NetworkId> current)
        {
            foreach (var state in current)
            {
                Debug.Log($"ApplyCollectionState {state.Key} {state.Value}");
                switch (state.Key)
                {
                    case CollectionState.Collected:
                        {
                            var collectable =
                                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<
                                    Collectable>(state.Value);
                            collectable.gameObject.SetActive(false);
                            break;
                        }
                    case CollectionState.Collecting:
                        {
                            if (current.TryGet(CollectionState.Collected, out var _))
                                break;

                            var collector =
                                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<
                                    Collector>(state.Value);

                            if (collector && collector.Object.HasInputAuthority)
                                collector.EnqueueForCollection(CollectableType, 1);

                            if (animator)
                                animator.SetBool(animator_collected_bool, true);

                            break;
                        }
                }
            }
        }

        internal void EnqueueForCollector(Collector collector)
        {
            Collected.Set(CollectionState.Collecting, collector.Object.Id);
        }
    }
}
