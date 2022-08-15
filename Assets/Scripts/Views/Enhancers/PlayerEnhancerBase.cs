using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Fusion;
using Fusion.KCC;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public abstract class PlayerEnhancerBase : NetworkBehaviour
    {
        [Inject] private readonly PlayerSpecsService playerSpecsService;

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
                var speedValue = 1 + groundKCC.KinemSpeedEnhancer;
                Debug.Log($"KinematicSpeed: {speedValue}");
                var jumpValue = 1 + groundKCC.JumpEnhancer;
                Debug.Log($"JumpMultiplier: {jumpValue}");

                playerSpecsService.HUDScreen.SpeedValue = speedValue.ToString("0.00");
                playerSpecsService.HUDScreen.JumpValue = jumpValue.ToString("0.00");
            }

        }
        protected virtual void EnhancementApplier(int count) { }
    }
}