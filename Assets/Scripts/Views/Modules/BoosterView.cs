using Example;
using Fusion;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class BoosterView : ModuleView<BoosterView>, IModuleView
    {
        protected override string HatchName => "back";
        protected override string EngageName => "engage";
        protected override float EngageTime => .1f;
        protected override bool ToggleAction(GameplayInput input) => input.Button3;
        protected override bool EngageAction(GameplayInput input) => input.Dash;

    }
}