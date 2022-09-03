using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class PlayerModulesView : NetworkBehaviour
    {
        [SerializeField] private Animator hatchesAnimator = null;

        private void OnHatchOpenRequest(string hatch, IModuleView module) =>
            StartCoroutine(nameof(ToggleHatchCoroutine), hatch);

        private void Start()
        {            
            foreach (var module in GetComponentsInChildren<IModuleView>()
                .Where(m => m.PrimaryModule == null))
                module.HatchOpenRequest += OnHatchOpenRequest;
        }
        private void OnDestroy()
        {
            foreach (var module in GetComponentsInChildren<IModuleView>()
                .Where(m => m.PrimaryModule == null))
                module.HatchOpenRequest -= OnHatchOpenRequest;
        }

        private void Update()
        {
            var speed = 2.0f * Time.deltaTime;
            var bodyDir = Vector3.RotateTowards(
                transform.forward,
                hatchesAnimator.gameObject.transform.parent.forward, speed, 0.0f);
            transform.rotation = Quaternion.LookRotation(bodyDir);
        }

        public IEnumerator ToggleHatchCoroutine(string hatch)
        {
            hatchesAnimator.SetBool(hatch, true);
            yield return new WaitForSeconds(1);
            hatchesAnimator.SetBool(hatch, false);
        }
    }
}