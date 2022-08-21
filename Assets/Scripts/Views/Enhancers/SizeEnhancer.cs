using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class SizeEnhancer : PlayerEnhancerBase
    {
        [Networked]
        public float SizeEnhancerValue { get; set; } = 1.0f;

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void EnhanceSizeRPC(float multiplier)
        {
            SizeEnhancerValue = multiplier;
        }

        protected override void EnhancementApplier(int count)
        {
            var enhancement = 1 + Step * Mathf.Min(count, MaxBalance);
            EnhanceSizeRPC(enhancement);
            playerSpecsService.HUDScreen.SizeValue = enhancement.ToString("0.00");
        }

        public override void Render()
        {
            transform.localScale = Vector3.one * SizeEnhancerValue;
        }
    }
}