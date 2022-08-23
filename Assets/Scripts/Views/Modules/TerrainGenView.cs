using Example;

namespace Assets.Scripts.Views
{
    public class TerrainGenView : ModuleView<TerrainGenView>, IModuleView
    {
        protected override string HatchName => "bottom";
        protected override bool ToggleAction(GameplayInput input) => input.Button0;
        protected override string AnimationReadyBool => null;
        protected override string EngageName => null;

        protected override void ToggleVisual(bool state)
        {
            base.ToggleVisual(state);
            
            var terrainMonitor = GetComponentInParent<PlayerWorldSpotMonitor>();
            if (terrainMonitor)
                terrainMonitor.ManualToggleGen(state);
        }

    }
}