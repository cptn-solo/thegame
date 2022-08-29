using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class SizeEnhancer : PlayerEnhancerBase
    {
        [Networked(
            OnChanged = nameof(OnChanged),
            OnChangedTargets = OnChangedTargets.InputAuthority)]
        public float SizeEnhancerValue { get; set; } = 1.0f;

        protected override void EnhancementApplier(int count, float multiplier)
        {
            // multiplier is skipped for size enhancement bc it is a source for this enhancement
            var enhancement = 1 + Step * Mathf.Min(count, MaxBalance);
            SizeEnhancerValue = enhancement;
        }
        public static void OnChanged(Changed<SizeEnhancer> changed) =>
            changed.Behaviour.HudUpdater(changed.Behaviour.SizeEnhancerValue);

        private void HudUpdater(float value) =>
            playerSpecsService.HUDScreen.SizeValue = value.ToString("0.00");

        public override void Render()
        {
            base.Render();
            transform.localScale = Vector3.one * SizeEnhancerValue;
        }
    }
}