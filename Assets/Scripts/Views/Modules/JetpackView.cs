using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class JetpackView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] jets;

        public void ToggleJet(bool toggle)
        {
            if (toggle)
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