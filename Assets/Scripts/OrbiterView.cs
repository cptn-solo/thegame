using Fusion;
using UnityEngine;

namespace Assets.Scripts
{
    public class OrbiterView : NetworkBehaviour
    {
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

        private void AttachSatellite(NetworkId curAncorRef)
        {
            var playerSatAnchorNTA =
                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<NetworkTransformAnchor>(
                    curAncorRef);
            var orbiterNTA = 
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