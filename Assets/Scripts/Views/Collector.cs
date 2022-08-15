using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Fusion;
using Fusion.KCC;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class Collector : NetworkBehaviour
    {

        [Inject] private readonly PlayerInventoryService playerInventory;
        
        [Networked(OnChanged = nameof(OnCollectedChanged)), Capacity(5)]
        public NetworkDictionary<CollectableType, int> Collected => default;

        private KCC kcc;
        private SpeedEnhancer speedEnhancer;
        private JumpEnhancer jumpEnhancer;

        private void Awake()
        {
            kcc = GetComponent<KCC>();
            kcc.OnCollisionEnter += Kcc_OnCollisionEnter;

            speedEnhancer = GetComponent<SpeedEnhancer>();
            jumpEnhancer = GetComponent<JumpEnhancer>();
        }

        private void OnDestroy()
        {
            kcc.OnCollisionEnter -= Kcc_OnCollisionEnter;
        }

        private void Kcc_OnCollisionEnter(KCC arg1, KCCCollision arg2)
        {
            if (arg2.NetworkObject.TryGetBehaviour<Collectable>(out var collectable))
                collectable.EnqueueForCollector(this);
        }

        public void EnqueueForCollection(CollectableType collectableType, int count)
        {
            Debug.Log($"EnqueueForCollection {collectableType} {count} {Object.HasStateAuthority}");

            if (!Object.HasStateAuthority)
                return;

            if (Collected.TryGet(collectableType, out var balance))
            {
                Collected.Set(collectableType, balance + count);
            }
            else
            {
                Collected.Add(collectableType, count);
            }
        }

        public static void OnCollectedChanged(Changed<Collector> changed)
        {
            var current = changed.Behaviour.Collected;

            changed.LoadOld();
            var prev = changed.Behaviour.Collected;

            if (!current.Equals(prev))
                changed.Behaviour.UpdateBalance(current);
        }

        private void UpdateBalance(NetworkDictionary<CollectableType, int> current)
        {
            if (speedEnhancer)
                speedEnhancer.Enhance(current);

            if (jumpEnhancer)
                jumpEnhancer.Enhance(current);

            if (!Object.HasInputAuthority)
                return;

            foreach(var collectable in current)
                playerInventory.SetCollectableBalance(collectable.Key, collectable.Value);
        }
    }
}