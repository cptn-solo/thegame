using Example;
using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Views
{
    public class PlayerModulesView : NetworkBehaviour
    {
        private const string animationHatchJetpack = "jetpack";
        private const string animationHatchBooster = "booster";
        private const string animationHatchDrone = "drone";

        private const string animationReadyBool = "ready";

        [SerializeField] private Animator hatchesAnimator = null;

        [SerializeField] private Animator boosterAnimator = null;
        [SerializeField] private Animator jetpackAnimator = null;
        [SerializeField] private Animator droneAnimator = null;

        [SerializeField] private JetpackView jetpackView;

        [Networked(OnChanged = nameof(OnJetpackReadyChange))]
        public NetworkBool JetpackReady { get; set; } = false;

        [Networked(OnChanged = nameof(OnBoosterReadyChange))]
        public NetworkBool BoosterReady { get; set; } = false;

        [Networked(OnChanged = nameof(OnDroneReadyChange))]
        public NetworkBool DroneReady { get; set; } = false;

        private static void OnJetpackReadyChange(Changed<PlayerModulesView> changed)
        {
            var current = changed.Behaviour.JetpackReady;
            changed.LoadOld();

            var old = changed.Behaviour.JetpackReady;

            if (!old.Equals(current))
                changed.Behaviour.ToggleJetpack(current);
        }
        private static void OnBoosterReadyChange(Changed<PlayerModulesView> changed)
        {
            var current = changed.Behaviour.BoosterReady;
            changed.LoadOld();

            var old = changed.Behaviour.BoosterReady;

            if (!old.Equals(current))
                changed.Behaviour.ToggleBooster(current);
        }
        private static void OnDroneReadyChange(Changed<PlayerModulesView> changed)
        {
            var current = changed.Behaviour.DroneReady;
            changed.LoadOld();

            var old = changed.Behaviour.DroneReady;

            if (!old.Equals(current))
                changed.Behaviour.ToggleDrone(current);
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void JetpackDeployRPC()
        {
            JetpackReady = !JetpackReady;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void BoosterDeployRPC()
        {
            BoosterReady = !BoosterReady;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void DroneDeployRPC()
        {
            DroneReady = !DroneReady;
        }

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

            if (JetpackReady && keyboard.spaceKey.wasPressedThisFrame)
                jetpackView.ToggleJet(true);
        }

        public void ToggleJetpack(bool state)
        {
            StartCoroutine(nameof(ToggleHatchetCoroutine), animationHatchJetpack);

            jetpackAnimator.SetBool(animationReadyBool, state);
        }

        private void ToggleBooster(bool state)
        {
            StartCoroutine(nameof(ToggleHatchetCoroutine), animationHatchBooster);

            boosterAnimator.SetBool(animationReadyBool, state);
        }

        private void ToggleDrone(bool state)
        {
            StartCoroutine(nameof(ToggleHatchetCoroutine), animationHatchDrone);

            droneAnimator.SetBool(animationReadyBool, state);
        }

        public IEnumerator ToggleHatchetCoroutine(string hatched)
        {
            hatchesAnimator.SetBool(hatched, true);
            yield return new WaitForSeconds(1);
            hatchesAnimator.SetBool(hatched, false);
        }
    }
}