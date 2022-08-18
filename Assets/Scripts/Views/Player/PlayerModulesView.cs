using Example;
using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Views
{
    public class PlayerModulesView : NetworkBehaviour
    {
        private const string animationDeployBool = "deploy";
        private const string animationRetractBool = "retract";
        private const string animationHatchJetpack = "джетпак";
        private const string animationHatchBooster = "booster";
        private const string animationReadyBool = "ready";

        private const string animationMoveForward = "Forward";

        [SerializeField] private Animator hatchesAnimator = null;
        
        [SerializeField] private Animator boosterAnimator = null;
        [SerializeField] private Animator jetpackAnimator = null;

        [SerializeField] private GameObject jetpackDeploy = null;
        [SerializeField] private GameObject jetpackMain = null;

        [Networked(OnChanged = nameof(OnJetpackDeployChange))]
        public NetworkBool JetpackDeploy { get; set; } = false;

        [Networked(OnChanged = nameof(OnBoosterReadyChange))]
        public NetworkBool BoosterReady { get; set; } = false;

        private static void OnJetpackDeployChange(Changed<PlayerModulesView> changed)
        {
            var current = changed.Behaviour.JetpackDeploy;
            changed.LoadOld();

            var old = changed.Behaviour.JetpackDeploy;

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
            JetpackDeploy = !JetpackDeploy;
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

        public void ToggleJetpack(bool deploy)
        {
            Debug.Log($"ToggleJetpack {deploy}");

            StartCoroutine(nameof(ToggleJetpackCoroutine), deploy);
        }

        public IEnumerator ToggleJetpackCoroutine(bool deploy)
        {   
            Debug.Log($"ToggleJetpackCoroutine {deploy}");

            ToggleDeployedState(deploy);
            yield return new WaitForSeconds(4);
            SwitchVisual(deploy);
        }

        private void ToggleBooster(bool state)
        {
            Debug.Log($"ToggleBooster {state}");

            hatchesAnimator.SetBool(animationMoveForward, false);
            hatchesAnimator.gameObject.transform.localRotation = Quaternion.identity;
            
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


        private void ToggleDeployedState(bool deploy)
        {
            Debug.Log($"ToggleDeployedState {deploy}");
            
            hatchesAnimator.SetBool(animationMoveForward, false);
            hatchesAnimator.gameObject.transform.localRotation = Quaternion.identity;
            hatchesAnimator.SetBool(animationHatchJetpack, true);

            if (deploy)
            {

                jetpackDeploy.SetActive(true);
                jetpackMain.SetActive(false);

                jetpackAnimator.SetBool(animationRetractBool, false);
                jetpackAnimator.SetBool(animationDeployBool, true);
            }
            else
            {
                jetpackDeploy.SetActive(true);
                jetpackMain.SetActive(false);

                jetpackAnimator.SetBool(animationRetractBool, true);
                jetpackAnimator.SetBool(animationDeployBool, false);
            }
        }

        private void SwitchVisual(bool deployed)
        {
            Debug.Log($"SwitchVisual {deployed}");

            jetpackAnimator.SetBool(animationDeployBool, false);
            jetpackAnimator.SetBool(animationRetractBool, false);
            hatchesAnimator.SetBool(animationHatchJetpack, false);

            if (deployed)
            {
                jetpackDeploy.SetActive(false);
                jetpackMain.SetActive(true);
            }
            else
            {
                jetpackDeploy.SetActive(false);
                jetpackMain.SetActive(false);
            }
        }
    }
}