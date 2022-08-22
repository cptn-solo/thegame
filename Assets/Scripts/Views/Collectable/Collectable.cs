using Assets.Scripts.Data;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{

    public class Collectable : NetworkBehaviour, IPredictedSpawnBehaviour
    {
        private const string animator_collected_bool = "collected_bool";

        [SerializeField] private Animator animator = null;

        private Rigidbody rb;

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
            rb = GetComponent<Rigidbody>();

            var infoComponent = GetComponentInChildren<CollectableInfo>();
            if (infoComponent)
                CollectableType = infoComponent.CollectableType;
        }

        private void OnCollisionEnter(Collision collision)
        {
            rb.useGravity = true;
            rb.mass = 1.0f;
        }

        internal void SetCollectedState()
        {
            if (Object.HasStateAuthority)
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

                            Runner.Despawn(collectable.Object);

                            break;
                        }
                    case CollectionState.Collecting:
                        {
                            if (current.TryGet(CollectionState.Collected, out var _))
                                break;

                            var collector =
                                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<
                                    Collector>(state.Value);

                            if (collector)
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
            Debug.Log($"EnqueueForCollector {collector.Object.Id} {collector.Object.HasStateAuthority} {Runner.IsServer}");

            if (Runner.IsServer)
                Collected.Set(CollectionState.Collecting, collector.Object.Id);

            //localCollectionState = CollectionState.Collecting;
            //collectorRef = collector.Object.Id;
        }

        void IPredictedSpawnBehaviour.PredictedSpawnSpawned()
        {
            Debug.Log($"PredictedSpawnSpawned {Object.Id}");
            Spawned();
        }

        void IPredictedSpawnBehaviour.PredictedSpawnUpdate()
        {
            FixedUpdateNetwork();
        }

        void IPredictedSpawnBehaviour.PredictedSpawnRender()
        {
            
        }

        void IPredictedSpawnBehaviour.PredictedSpawnFailed()
        {
            Debug.Log($"PredictedSpawnFailed {Object.Id}");
            Runner.Despawn(Object, true);
        }

        void IPredictedSpawnBehaviour.PredictedSpawnSuccess()
        {
            Debug.Log($"PredictedSpawnSuccess {Object.Id}");
        }

        public override void Spawned()
        {
        }

    }
}
