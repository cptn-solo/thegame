using Assets.Scripts.Data;
using Fusion.KCC;

namespace Assets.Scripts.Views
{
    public class CollectableKCCProcessor : KCCProcessor
    {
        private Collectable collectable;

        private void Awake()
        {
            collectable = GetComponent<Collectable>();
        }

        public override void OnEnter(KCC kcc, KCCData data)
        {
            if (kcc.TryGetComponent<Collector>(out var collector))
            {
                collectable.EnqueueForCollector();
                
                if (collector.Object.InputAuthority == collector.Runner.LocalPlayer)
                    collector.EnqueueForCollection(collectable.CollectableType, 1);

            }
        }
    }
}