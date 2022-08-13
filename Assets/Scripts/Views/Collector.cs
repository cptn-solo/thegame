using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Fusion;
using Zenject;

namespace Assets.Scripts.Views
{
    public class Collector : NetworkBehaviour
    {
        [Inject] private readonly PlayerInventoryService playerInventory;

        [Networked, Capacity(5)] public NetworkDictionary<CollectableType, int> Collected => default;

        public void Collect(CollectableType collectableType, int count)
        {
            playerInventory.AddCollectableItem(collectableType, count);
            if (Collected.TryGet(collectableType, out _))
            {
                Collected.Set(collectableType, playerInventory.Collectables[collectableType]);
            }
            else
            {
                Collected.Add(collectableType, playerInventory.Collectables[collectableType]);
            }
        }
    }
}