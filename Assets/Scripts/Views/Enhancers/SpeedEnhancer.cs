using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class SpeedEnhancer : PlayerEnhancerBase
    {
		[Networked(OnChanged = nameof(OnChanged))]
		public float SpeedEnhancerValue { get; set; } = 1.0f;

        protected override void EnhancementApplier(int count, float multiplier)
        {
            var enhancement = (1 + Step * Mathf.Min(count, MaxBalance)) * multiplier;
            SpeedEnhancerValue = enhancement;
        }

        public static void OnChanged(Changed<SpeedEnhancer> changed) =>
            changed.Behaviour.HudUpdater(changed.Behaviour.SpeedEnhancerValue);

        private void HudUpdater(float value) =>
            playerSpecsService.HUDScreen.SpeedValue = value.ToString("0.00");

        public void Enhance(KCC kcc, KCCData data) =>
            data.SpeedEnhancerValue = SpeedEnhancerValue;
    }
}