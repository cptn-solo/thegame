using Assets.Scripts.Data;
using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class Collector : NetworkBehaviour
    {
        [Networked, Capacity(5)] public NetworkDictionary<CollectableType, int> Collected => default;

        public void Collect(CollectableType collectableType, int count)
        {
            if (Collected.TryGet(collectableType, out var typeCount))
            {
                Collected.Set(collectableType, typeCount + count);
            }
            else
            {
                Collected.Add(collectableType, count);
            }

            Debug.Log($"Collected {collectableType}, now have {Collected.Get(collectableType)}");
        }
    }
}