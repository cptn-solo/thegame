using Assets.Scripts.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Services.App
{
    public class PlayerInventoryService : MonoBehaviour
    {
        private readonly Dictionary<CollectableType, int> collectables = new();

        public Dictionary<CollectableType, int> Collectables => collectables;

        public event UnityAction<KeyValuePair<CollectableType, int>> OnInventoryChange;

        public void Initialize()
        {
            collectables.Add(CollectableType.Box, 0);
            collectables.Add(CollectableType.Diamond, 0);
            collectables.Add(CollectableType.Ring, 0);
            collectables.Add(CollectableType.Frame, 0);
            collectables.Add(CollectableType.SnowFlake, 0);
        }

        public void SetCollectableBalance(CollectableType collectableType, int balance)
        {
            Debug.Log($"SetCollectableBalance {collectableType} {balance}");
            collectables[collectableType] = balance;
            OnInventoryChange?.Invoke(new KeyValuePair<CollectableType, int>(collectableType, balance));
        }

    }
}
