using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Views
{
    public class PlayerModulesView : NetworkBehaviour
    {
        [SerializeField] private JetpackView jetpackView;
        [SerializeField] private BoosterView boosterView;
        [SerializeField] private DroneView droneView;

        [SerializeField] private Animator hatchesAnimator = null;

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void JetpackDeployRPC() => jetpackView.Toggle(OnHatchOpenRequest);

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void BoosterDeployRPC() => boosterView.Toggle(OnHatchOpenRequest);

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void DroneDeployRPC() => droneView.Toggle(OnHatchOpenRequest);

        private void OnHatchOpenRequest(string hatch, IModuleView module) =>
            StartCoroutine(nameof(ToggleHatchCoroutine), hatch);

        private void Update()
        {
            //transform.localRotation = 
            //    hatchesAnimator.gameObject.transform.parent.localRotation;
            var speed = 2.0f * Time.deltaTime;
            var bodyDir = Vector3.RotateTowards(
                transform.forward,
                hatchesAnimator.gameObject.transform.parent.forward, speed, 0.0f);
            transform.rotation = Quaternion.LookRotation(bodyDir);

            if (!Object.HasInputAuthority)
                return;

            Keyboard keyboard = Keyboard.current;
            if (keyboard.digit1Key.wasPressedThisFrame)
                JetpackDeployRPC();

            if (keyboard.digit2Key.wasPressedThisFrame)
                BoosterDeployRPC();

            if (keyboard.digit3Key.wasPressedThisFrame)
                DroneDeployRPC();

            if (jetpackView.ModuleReady && keyboard.spaceKey.wasPressedThisFrame)
                jetpackView.Engage(true);
        }

        public IEnumerator ToggleHatchCoroutine(string hatch)
        {
            hatchesAnimator.SetBool(hatch, true);
            yield return new WaitForSeconds(1);
            hatchesAnimator.SetBool(hatch, false);
        }
    }
}