using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class JumpEnhancer : PlayerEnhancerBase
    {
        [Networked(
            OnChanged = nameof(OnChanged),
            OnChangedTargets = OnChangedTargets.InputAuthority)]
        public float JumpEnhancerValue { get; set; } = 1.0f;

        protected override void EnhancementApplier(int count, float multiplier)
        {
            var enhancement = (1 + Step * Mathf.Min(count, MaxBalance)) * multiplier;
            JumpEnhancerValue = enhancement;
        }

        public static void OnChanged(Changed<JumpEnhancer> changed) =>
            changed.Behaviour.HudUpdater(changed.Behaviour.JumpEnhancerValue);

        private void HudUpdater(float value) =>
            playerSpecsService.HUDScreen.JumpValue = value.ToString("0.00");

        public void Enhance(KCC kcc, KCCData data)
        {
            if (JumpEnhancerValue != 0.0f)
                data.JumpEnhancerValue = JumpEnhancerValue;
        }

    }
}