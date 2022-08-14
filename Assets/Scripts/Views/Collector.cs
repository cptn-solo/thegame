using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Fusion;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class Collector : NetworkBehaviour
    {

        [Inject] private readonly PlayerInventoryService playerInventory;
                
        [Networked(OnChanged = nameof(OnCollectedChanged)), Capacity(5)]
        public NetworkDictionary<CollectableType, int> Collected => default;

        public void EnqueueForCollection(CollectableType collectableType, int count)
        {
            Debug.Log($"EnqueueForCollection {collectableType} {count} {Object.HasInputAuthority}");

            if (!Object.HasInputAuthority)
                return;

            var currentBalance = playerInventory.Collectables[collectableType];

            Collected.Set(collectableType, currentBalance + count);
        }

        public static void OnCollectedChanged(Changed<Collector> changed)
        {
            if (!changed.Behaviour.Object.HasInputAuthority)
                return;

            var current = changed.Behaviour.Collected;

            changed.LoadOld();
            var prev = changed.Behaviour.Collected;

            if (!current.Equals(prev))
                changed.Behaviour.UpdateBalance(current);

            foreach (var x in current)
                Debug.Log($"OnCollectedChanged {x.Key} {x.Value}");
        }

        private void UpdateBalance(NetworkDictionary<CollectableType, int> current)
        {
            foreach(var collectable in current)
                playerInventory.SetCollectableBalance(collectable.Key, collectable.Value);
        }
    }
}