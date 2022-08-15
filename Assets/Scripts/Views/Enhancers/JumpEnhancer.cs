using UnityEngine;

namespace Assets.Scripts.Views
{
    public class JumpEnhancer : PlayerEnhancerBase
    {
        protected override void EnhancementApplier(int count) =>
            GroundKCC.JumpEnhancer = Step * Mathf.Min(count, MaxBalance);
    }
}