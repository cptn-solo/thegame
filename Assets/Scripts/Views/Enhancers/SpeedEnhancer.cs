using UnityEngine;

namespace Assets.Scripts.Views
{
    public class SpeedEnhancer : PlayerEnhancerBase
    {
        protected override void EnhancementApplier(int count) =>
            GroundKCC.KinemSpeedEnhancer = Step * Mathf.Min(count, MaxBalance);
    }
}