using Assets.Scripts.Data;
using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public abstract class PlayerEnhancerBase : NetworkBehaviour
    {
        [SerializeField] private CollectableType collectable = CollectableType.Box;
        [SerializeField] private int maxBalance = 10;
        [SerializeField] private float step = .1f;

        private GroundKCCProcessor groundKCC;
        protected float Step => step;
        protected int MaxBalance => maxBalance;
        protected GroundKCCProcessor GroundKCC => groundKCC;

        private void Start()
        {
            if (TryGetBehaviour<KCC>(out var kcc))
                groundKCC = kcc.GetProcessor<GroundKCCProcessor>();
        }

        public void Enhance(NetworkDictionary<CollectableType, int> balance)
        {
            if (!groundKCC)
                return;

            if (balance.TryGet(collectable, out var count))
            {
                EnhancementApplier(count);
                Debug.Log($"KinematicSpeed: {groundKCC.KinematicSpeed + groundKCC.KinemSpeedEnhancer}");
                Debug.Log($"JumpMultiplier: {groundKCC.JumpMultiplier + groundKCC.JumpEnhancer}");
            }

        }
        protected virtual void EnhancementApplier(int count) { }
    }
}