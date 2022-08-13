using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    [RequireComponent(typeof(NetworkTransformAnchor), typeof(NetworkObject))]
    public class AttachableView : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(AnchorRefChanged))]
        private NetworkId AnchorRef { get; set; }

        private bool resetPositionAfterAttach = true;

        public void InitForAnchorRef(NetworkId anchorRef, bool resetPosition = true)
        {
            resetPositionAfterAttach = resetPosition;
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

                if (resetPositionAfterAttach)
                    childNta.transform.SetPositionAndRotation(
                        parentNta.transform.position,
                        parentNta.transform.rotation);
            }
        }

    }
}
