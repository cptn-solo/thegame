using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Fusion;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public struct CollectedItems : INetworkStruct
    {
        [Networked]
        public NetworkDictionary<CollectableType, int> Items => default;
        public int ChangeCount;

    }
    public class Collector : NetworkBehaviour
    {

        [Inject] private readonly PlayerInventoryService playerInventory;
        [Inject] private readonly PlayerSpecsService playerSpecsService;

        [Networked(OnChanged = nameof(CollectedOnChange))]
        private CollectedItems Collected { get; set; }
        private int changeCountOld;

        [SerializeField] private LayerMask collectableLayer;

        private IPlayerEnhancer[] enhancers;
        

        private void Awake() =>
            enhancers = GetComponents<IPlayerEnhancer>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(collectableLayer))
                other.gameObject.GetComponentInParent<Collectable>().EnqueueForCollector(this);
        }

        public void EnqueueForCollection(CollectableType collectableType, int count)
        {
            if (!Runner.IsServer)
                return;

            var c = Collected;

            if (c.Items.TryGet(collectableType, out var balance))
            {
                c.Items.Set(collectableType, balance + count);
            }
            else
            {
                c.Items.Add(collectableType, count);
            }
            c.ChangeCount++;

            Collected = c;
        }
        public int GetRandomDropCount()
        {
            var totalCount = Collected.Items.Where(c => c.Value > 0).Select(c => c.Value).Sum();
            if (totalCount == 0)
                return 0;

            return Random.Range(1, Mathf.FloorToInt(Mathf.Sqrt(totalCount)));
        }
        public bool TryGetCollectableTypeToDrop(CollectableType collectableType, bool dropInstantly = false)
        {
            var availableCount = Collected.Items.Where(c => c.Key == collectableType).Select(c => c.Value).Sum();

            if (dropInstantly && availableCount > 0)
                EnqueueForCollection(collectableType, -1);

            return availableCount > 0;
        }

        public bool TryGetCollectableToDrop(out CollectableType collectableType)
        {
            collectableType = default;

            var available = Collected.Items.Where(c => c.Value > 0).Select(c => c.Key).ToArray();
            if (available.Length > 0)
            {
                collectableType = available[Random.Range(0, available.Length)];
                return true;
            }

            return false;
        }


        public override void FixedUpdateNetwork()
        {
            if (Collected.ChangeCount == changeCountOld)
                return;

            if (Runner.IsServer)
            {
                var multiplier = 1.0f;
                if (TryGetComponent<SizeEnhancer>(out var sizeEnhancer))
                {
                    sizeEnhancer.Enhance(Collected.Items);
                    multiplier = sizeEnhancer.SizeEnhancerValue;
                }

                foreach (var enhancer in enhancers.Where(x => x.GetType() != typeof(SizeEnhancer)))
                    enhancer.Enhance(Collected.Items, multiplier);
            }

            if (Object.HasInputAuthority)
            {
                foreach (var collected in Collected.Items)
                    if (!playerInventory.Compare(collected.Key, collected.Value))
                        playerInventory.SetCollectableBalance(collected.Key, collected.Value);
            }
            
            changeCountOld = Collected.ChangeCount;
        }

        private static void CollectedOnChange(Changed<Collector> changed)
        {
            changed.Behaviour.UpdatePlayerInfo(changed.Behaviour.Collected.Items);
        }

        private void UpdatePlayerInfo(NetworkDictionary<CollectableType, int> collected)
        {
            if (Object.HasInputAuthority)
                playerSpecsService.Score = collected.Sum(x => x.Value);
        }
    }
}