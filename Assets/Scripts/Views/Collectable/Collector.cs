using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Fusion;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class Collector : NetworkBehaviour
    {

        [Inject] private readonly PlayerInventoryService playerInventory;
        [Inject] private readonly PlayerSpecsService playerSpecsService;

        [Networked(OnChanged = nameof(CollectedOnChange)), Capacity(5)]
        public NetworkDictionary<CollectableType, int> Collected => default;

        [Networked]
        public int ChangeCount { get; set; }

        private int changeCountOld = 0;
 
        [SerializeField] private LayerMask collectableLayer;

        private IPlayerEnhancer[] enhancers;

        private void Awake()
        {
            enhancers = GetComponents<IPlayerEnhancer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(collectableLayer))
                other.gameObject.GetComponentInParent<Collectable>().EnqueueForCollector(this);
        }

        public void EnqueueForCollection(CollectableType collectableType, int count)
        {
            if (!Runner.IsServer)
                return;

            if (Collected.TryGet(collectableType, out var balance))
            {
                Collected.Set(collectableType, balance + count);
            }
            else
            {
                Collected.Add(collectableType, count);
            }
            ChangeCount++;
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer && ChangeCount != changeCountOld)
            {
                Debug.Log($"C FUN server, {ChangeCount} {changeCountOld}");
                var multiplier = 1.0f;
                if (TryGetComponent<SizeEnhancer>(out var sizeEnhancer))
                    multiplier = sizeEnhancer.SizeEnhancerValue;

                foreach (var enhancer in enhancers)
                    enhancer.Enhance(Collected, multiplier);
            }
            if (Object.HasInputAuthority && ChangeCount != changeCountOld)
            {
                Debug.Log($"C FUN IA, {ChangeCount} {changeCountOld}");
                foreach (var collected in Collected)
                    if (!playerInventory.Compare(collected.Key, collected.Value))
                        playerInventory.SetCollectableBalance(collected.Key, collected.Value);
            }
            changeCountOld = ChangeCount;
        }
        private static void CollectedOnChange(Changed<Collector> changed)
        {
            changed.Behaviour.UpdatePlayerInfo(changed.Behaviour.Collected);
        }

        private void UpdatePlayerInfo(NetworkDictionary<CollectableType, int> collected)
        {
            if (Object.HasInputAuthority)
                playerSpecsService.Score = collected.Sum(x => x.Value);
        }
    }
}