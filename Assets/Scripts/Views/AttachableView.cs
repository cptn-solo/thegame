using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    [RequireComponent(typeof(NetworkTransformAnchor), typeof(NetworkObject))]
    public class AttachableView : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(AnchorRefChanged))]
        private NetworkId AnchorRef { get; set; }

        public void InitForAnchorRef(NetworkId anchorRef)
        {
            AnchorRef = anchorRef;
        }

        protected static void AnchorRefChanged(Changed<AttachableView> changed)
        {
            changed.Behaviour.Attach(changed.Behaviour.AnchorRef);
        }

        private void Attach(NetworkId anchorRef)
        {
            var parentNta =
                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<
                    NetworkTransformAnchor>(anchorRef);
            var childNta =
                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<
                    NetworkTransformAnchor>(Object.Id);

            if (parentNta && childNta)
            {
                childNta.transform.SetParent(
                    parentNta.transform);

                childNta.transform.SetPositionAndRotation(
                    parentNta.transform.position,
                    parentNta.transform.rotation);
            }
        }

    }
}
