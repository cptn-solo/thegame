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

        [Networked]
        public NetworkDictionary<CollectionState, NetworkId> Collected => default;

        private int oldCount = 0;

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
            if (Object && Object.HasStateAuthority)
                Collected.Set(CollectionState.Collecting, collector.Object.Id);
        }
        public override void FixedUpdateNetwork()
        {
            if (!Runner.IsServer && !Runner.IsResimulation)
                return;

            if (Collected.Count > oldCount)
            {
                oldCount = Collected.Count;
                ApplyCollectionState(Collected);
            }
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
