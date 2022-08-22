using Example;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class JetpackView : ModuleView<JetpackView>, IModuleView
    {
        protected override string HatchName => "sides";
        protected override bool ToggleAction(GameplayInput input) => input.Button2;
        protected override bool EngageAction(GameplayInput input) => input.Jump;

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