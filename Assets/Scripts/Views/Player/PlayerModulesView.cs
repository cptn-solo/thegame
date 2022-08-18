using Example;
using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Views
{
    public class PlayerModulesView : NetworkBehaviour
    {
        private const string animationHatchJetpack = "джетпак";
        private const string animationHatchBooster = "booster";

        private const string animationReadyBool = "ready";

        [SerializeField] private Animator hatchesAnimator = null;

        [SerializeField] private Animator boosterAnimator = null;
        [SerializeField] private Animator jetpackAnimator = null;

        [Networked(OnChanged = nameof(OnJetpackReadyChange))]
        public NetworkBool JetpackReady { get; set; } = false;

        [Networked(OnChanged = nameof(OnBoosterReadyChange))]
        public NetworkBool BoosterReady { get; set; } = false;

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

        private Player player = null;

        private void Awake()
        {
            player = transform.parent.gameObject.GetComponent<ThirdPersonPlayer>();
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

        }

        public void ToggleJetpack(bool state)
        {
            Debug.Log($"ToggleJetpack {state}");

            StartCoroutine(nameof(ToggleHatchetCoroutine), animationHatchJetpack);

            jetpackAnimator.SetBool(animationReadyBool, state);
        }

        private void ToggleBooster(bool state)
        {
            Debug.Log($"ToggleBooster {state}");

            StartCoroutine(nameof(ToggleHatchetCoroutine), animationHatchBooster);

            boosterAnimator.SetBool(animationReadyBool, state);
        }

        public IEnumerator ToggleHatchetCoroutine(string hatched)
        {
            Debug.Log($"ToggleHatchetCoroutine {hatched}");

            hatchesAnimator.SetBool(hatched, true);
            yield return new WaitForSeconds(2);
            hatchesAnimator.SetBool(hatched, false);
        }
    }
}