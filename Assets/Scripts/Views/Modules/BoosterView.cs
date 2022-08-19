using Fusion;

namespace Assets.Scripts.Views
{
    public class BoosterView : ModuleView<BoosterView>, IModuleView
    {
        protected override string HatchName => "back";

        [Networked(OnChanged = nameof(OnChanged))]
        public override NetworkBool ModuleReady { get; set; } = false;

        private static void OnChanged(Changed<BoosterView> changed) =>
            OnModuleReadyChange(changed);

    }
}