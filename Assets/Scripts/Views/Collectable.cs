using Assets.Scripts.Data;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{

    public class Collectable : NetworkBehaviour
    {
        private const string animator_collected_bool = "collected_bool";

        [SerializeField] private Animator animator = null;

        public CollectableType CollectableType { get; private set; }

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
            var infoComponent = GetComponentInChildren<CollectableInfo>();
            if (infoComponent)
                CollectableType = infoComponent.CollectableType;
        }

        public void ApplyCollectionState(CollectionState currentState)
        {
            switch (currentState)
            {
                case CollectionState.Collected:
                    {
                        gameObject.SetActive(false);
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

        internal void EnqueueForCollector(Collector collector)
        {
            if (!Collected.TryGet(Object.Id, out _))
            {
                SetCollectedState(CollectionState.Collecting);

                if (collector.Object.InputAuthority == collector.Runner.LocalPlayer)
                    collector.EnqueueForCollection(CollectableType, 1);
            }
        }

        internal void SetCollectedState(CollectionState state) =>
            Collected.Set(Object.Id, state);
    }
}
