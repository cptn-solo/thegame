using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Example;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class Collector : NetworkBehaviour
    {

        [Inject] private readonly PlayerInventoryService playerInventory;

        [SerializeField] private GameObject[] collectablePrefabs;
        
        private float spawnTimer = 0.0f;
        private readonly float spawnCooldown = 1.0f;
        
        [Networked, Capacity(5)] public NetworkDictionary<CollectableType, int> Collected => default;

        public void EnqueueForCollection(CollectableType collectableType, int count)
        {
            playerInventory.AddCollectableItem(collectableType, count);
            Collected.Set(collectableType, playerInventory.Collectables[collectableType]);
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.TryGetInputForPlayer(Object.InputAuthority, out GameplayInput input) && 
                input.RMB && spawnTimer <= 0.0f &&
                playerInventory.Collectables[CollectableType.Box] > 0)
            {
                spawnTimer = spawnCooldown;
                Runner.Spawn(collectablePrefabs[(int)CollectableType.Box], transform.position + transform.forward * 2.0f);
            }                
        }

        private void Update()
        {
            if (spawnTimer > 0.0f)
                spawnTimer -= Time.deltaTime;
        }

    }
}