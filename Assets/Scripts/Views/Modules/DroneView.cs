using Fusion;

namespace Assets.Scripts.Views
{
    public class DroneView : ModuleView<DroneView>, IModuleView
    {
        protected override string HatchName => "front";

        [Networked(OnChanged = nameof(OnChanged))]
        public override NetworkBool ModuleReady { get; set; } = false;

        private static void OnChanged(Changed<DroneView> changed) =>
            OnModuleReadyChange(changed);
    }
}