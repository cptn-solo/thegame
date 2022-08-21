using UnityEngine;

namespace Assets.Scripts.Views
{
    public class BallView : ModuleView<BallView>, IModuleView
    {
        [SerializeField] private BallLauncherView ballLauncher = null;
        protected override string HatchName => "top";
        protected override string EngageName => "engage";
        protected override float EngageTime => 2.0f;


        protected override void EngageVisual(bool engage)
        {
            if (!engage)
                ballLauncher.Park();

            base.EngageVisual(engage);

            if (engage)
                ballLauncher.Launch(EngageDir);
        }
    }
}