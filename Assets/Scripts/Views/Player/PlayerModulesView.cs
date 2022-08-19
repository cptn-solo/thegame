using Example;
using Fusion;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class PlayerModulesView : NetworkBehaviour
    {
        [SerializeField] private JetpackView jetpackView;
        [SerializeField] private BoosterView boosterView;
        [SerializeField] private DroneView droneView;

        [SerializeField] private Animator hatchesAnimator = null;

        private TickTimer moduleToggleTimer;

        private void OnHatchOpenRequest(string hatch, IModuleView module) =>
            StartCoroutine(nameof(ToggleHatchCoroutine), hatch);

        private void Start()
        {
            jetpackView.HatchOpenRequest += OnHatchOpenRequest;
            boosterView.HatchOpenRequest += OnHatchOpenRequest;
            droneView.HatchOpenRequest += OnHatchOpenRequest;
        }
        private void OnDestroy()
        {
            jetpackView.HatchOpenRequest -= OnHatchOpenRequest;
            boosterView.HatchOpenRequest -= OnHatchOpenRequest;
            droneView.HatchOpenRequest -= OnHatchOpenRequest;
        }

        private void Update()
        {
            var speed = 2.0f * Time.deltaTime;
            var bodyDir = Vector3.RotateTowards(
                transform.forward,
                hatchesAnimator.gameObject.transform.parent.forward, speed, 0.0f);
            transform.rotation = Quaternion.LookRotation(bodyDir);
        }
        private void InitModuleToggleTimer() =>
            moduleToggleTimer = TickTimer.CreateFromSeconds(Runner, .3f);

        public override void FixedUpdateNetwork()
        {
            if (!moduleToggleTimer.ExpiredOrNotRunning(Runner))
                return;

            if (Runner.Stage == SimulationStages.Forward &&
                Runner.TryGetInputForPlayer<GameplayInput>(Object.InputAuthority, out var input))
            {
                if (jetpackView.ModuleReady && input.Jump)
                    jetpackView.Engage(true);

                if (boosterView.ModuleReady && input.Dash)
                    boosterView.Engage(true);

                if (input.Button1)
                    jetpackView.Toggle();

                if (input.Button2)
                    boosterView.Toggle();

                if (input.Button3)
                    droneView.Toggle();

                if (input.Jump ||
                    input.Button1 ||
                    input.Button2 ||
                    input.Button3)
                {
                    InitModuleToggleTimer();
                }
            }
        }

        public IEnumerator ToggleHatchCoroutine(string hatch)
        {
            hatchesAnimator.SetBool(hatch, true);
            yield return new WaitForSeconds(1);
            hatchesAnimator.SetBool(hatch, false);
        }
    }
}