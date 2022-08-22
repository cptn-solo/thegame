using Example;

namespace Assets.Scripts.Views
{
    public class DroneView : ModuleView<DroneView>, IModuleView
    {
        protected override string HatchName => "front";
        protected override bool ToggleAction(GameplayInput input) => input.Button4;

    }
}