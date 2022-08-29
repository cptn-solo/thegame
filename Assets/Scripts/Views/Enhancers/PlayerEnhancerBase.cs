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

        public void Enhance(NetworkDictionary<CollectableType, int> balance, float multiplier = 1)
        {
            if (balance.TryGet(collectable, out var count))
            {
                EnhancementApplier(count, multiplier);
            }
            else if (multiplier != 1)
            {   // enhance anyway if multiplier isn't 1
                EnhancementApplier(1, multiplier);
            }
        }

        protected virtual void EnhancementApplier(int count, float multiplier = 1.0f) { }
    }
}