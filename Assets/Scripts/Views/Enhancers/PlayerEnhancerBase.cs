using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Fusion;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public abstract class PlayerEnhancerBase : NetworkBehaviour, IPlayerEnhancer
    {
        [Inject] protected readonly PlayerSpecsService playerSpecsService;

        [SerializeField] private CollectableType collectable = CollectableType.Box;
        [SerializeField] private int maxBalance = 10;
        [SerializeField] private float step = .1f;

        protected float Step => step;
        protected int MaxBalance => maxBalance;

        public void Enhance(NetworkDictionary<CollectableType, int> balance)
        {
            if (balance.TryGet(collectable, out var count))
                EnhancementApplier(count);
        }
        protected virtual void EnhancementApplier(int count) { }
    }
}