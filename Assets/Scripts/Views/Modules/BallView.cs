using Example;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class BallView : ModuleView<BallView>, IModuleView
    {
        [SerializeField] private BallLauncherView ballLauncher = null;
        
        private KCC kcc;

        protected override string HatchName => "top";
        protected override string EngageName => "engage";
        protected override float EngageTime => .5f;
        protected override bool ToggleAction(GameplayInput input) => input.Button1;
        protected override bool EngageAction(GameplayInput input) => input.LMB;
        protected override Vector3 InputEngageDirection => kcc.FixedData.ShotDirection;


        private void Start()
        {
            kcc = GetComponentInParent<KCC>();
        }

        protected override void EngageVisual(bool engage)
        {
            ballLauncher.Park();

            base.EngageVisual(engage);

            if (engage)
                ballLauncher.Launch(EngageDir);
        }
    }
}