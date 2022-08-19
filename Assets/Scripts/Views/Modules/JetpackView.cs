using Fusion;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class JetpackView : ModuleView<JetpackView>, IModuleView
    {
        protected override string HatchName => "sides";
        
        [Networked(OnChanged = nameof(OnChanged))]
        public override NetworkBool ModuleReady { get; set; } = false;

        private static void OnChanged(Changed<JetpackView> changed) => 
            OnModuleReadyChange(changed);

        [SerializeField] private ParticleSystem[] jets;

        public override void Engage(bool engage)
        {
            if (engage)
                StartCoroutine(nameof(ToggleJetsVisual));
            else
                foreach (var jet in jets)
                    jet.Stop();
        }

        public IEnumerator ToggleJetsVisual()
        {
            foreach (var jet in jets)
                jet.Play();

            yield return new WaitForSeconds(.3f);

            foreach (var jet in jets)
                jet.Stop();
        }

    }
}