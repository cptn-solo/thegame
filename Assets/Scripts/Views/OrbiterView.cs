using Fusion;
using UnityEngine;

namespace Assets.Scripts
{
    public class OrbiterView : NetworkBehaviour
    {

        [SerializeField] private NetworkMecanimAnimator networkAnimator;


        private NetworkTransformAnchor playerSatAnchorNTA;
        private NetworkTransformAnchor orbiterNTA;
        

        [Networked(OnChanged = nameof(AnchorRefChanged))]
        private NetworkId AnchorRef { get; set; }

        public void InitSatelliteForAnchorRef(NetworkId anchorRef)
        {
            AnchorRef = anchorRef;
        }

        protected static void AnchorRefChanged(Changed<OrbiterView> changed)
        {
            changed.Behaviour.AttachSatellite(changed.Behaviour.AnchorRef);
        }

        private void Start()
        {
            networkAnimator.Animator.SetBool("test_bool_param", false);
            //networkAnimator.Animator.StartPlayback();
        }


        private void AttachSatellite(NetworkId curAncorRef)
        {
            playerSatAnchorNTA =
                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<NetworkTransformAnchor>(
                    curAncorRef);
            orbiterNTA = 
                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<NetworkTransformAnchor>(
                    Object.Id);

            if (playerSatAnchorNTA && orbiterNTA)
            {
                orbiterNTA.transform.SetParent(
                    playerSatAnchorNTA.transform);

                orbiterNTA.transform.SetPositionAndRotation(
                    playerSatAnchorNTA.transform.position,
                    playerSatAnchorNTA.transform.rotation);
                
                
            }
        }
    }
}