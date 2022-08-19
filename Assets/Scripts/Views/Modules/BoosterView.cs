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
    }
}