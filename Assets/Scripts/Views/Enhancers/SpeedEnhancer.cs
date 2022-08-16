using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class SpeedEnhancer : PlayerEnhancerBase
    {
		[Networked]
		public float SpeedEnhancerValue { get; set; } = 1.0f;


		[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
		public void EnhanceSpeedRPC(float multiplier)
		{
			SpeedEnhancerValue = multiplier;
		}

		protected override void EnhancementApplier(int count)
        {
            var enhancement = 1 + Step * Mathf.Min(count, MaxBalance);
            EnhanceSpeedRPC(enhancement);
            playerSpecsService.HUDScreen.SpeedValue = enhancement.ToString("0.00");
        }
		
		public void Enhance(KCC kcc, KCCData data)
		{
			data.SpeedEnhancerValue = SpeedEnhancerValue;
		}
	}
}