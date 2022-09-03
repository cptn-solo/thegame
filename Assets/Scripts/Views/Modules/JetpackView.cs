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
        protected override float EngageTime => .1f;

        [SerializeField] private ParticleSystem[] jets;
        [SerializeField] private Vector3 impulse = new Vector3(0, 20.0f, 10.0f);

        protected override void EngageVisual(bool engage)
        {
            if (engage)
            {
                StartCoroutine(nameof(ToggleJetsVisual));
                kcc.AddExternalVelocity(impulse);
            }
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