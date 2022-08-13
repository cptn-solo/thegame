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

        public void EnqueueForCollection(CollectableType collectableType, int count)
        {
            playerInventory.AddCollectableItem(collectableType, count);
            Collected.Set(collectableType, playerInventory.Collectables[collectableType]);
        }
    }
}