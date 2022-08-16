using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class JumpEnhancer : PlayerEnhancerBase
    {
        [Networked]
        public float JumpEnhancerValue { get; set; } = 1.0f;

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void EnhanceJumpRPC(float multiplier)
        {
            JumpEnhancerValue = multiplier;
        }

        protected override void EnhancementApplier(int count)
        {
            var enhancement = 1 + Step * Mathf.Min(count, MaxBalance);
            EnhanceJumpRPC(enhancement);
            playerSpecsService.HUDScreen.JumpValue = enhancement.ToString("0.00");
        }

        public void Enhance(KCC kcc, KCCData data)
        {
            if (JumpEnhancerValue != 0.0f)
            {
                data.JumpEnhancerValue = JumpEnhancerValue;
            }
        }

    }
}