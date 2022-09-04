using Example;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class MagnetBeamView : ModuleView<MagnetBeamView>, IModuleView
    {
        [SerializeField] private BallLauncherView beamLauncher = null;

        private BallView ballView;

        public override IModuleView PrimaryModule => ballView;
        protected override string EngageName => "engage2";
        protected override float EngageTime => .5f;
        protected override bool EngageAction(GameplayInput input) => input.RMB;
        protected override Vector3 InputEngageDirection => kcc.FixedData.Shot2Direction;

        protected override void OnAwake()
        {
            base.OnAwake();
            ballView = GetComponent<BallView>();
        }
        
        private void Start()
        {
            kcc = GetComponentInParent<KCC>();
        }

        protected override void EngageVisual(bool engage)
        {
            beamLauncher.Park();

            base.EngageVisual(engage);

            if (engage)
                beamLauncher.Launch(EngageDir, true);
        }

    }
}