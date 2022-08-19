using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class JetpackView : ModuleView<JetpackView>, IModuleView
    {
        protected override string HatchName => "sides";

        [SerializeField] private ParticleSystem[] jets;

        protected override void EngageVisual(bool engage)
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