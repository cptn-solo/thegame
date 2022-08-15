using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public enum AttachableOffsetKeys
    {
        Position, RotationDirection
    }

    [RequireComponent(typeof(NetworkTransformAnchor), typeof(NetworkObject))]
    public class AttachableView : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(AnchorRefChanged))]
        private NetworkId AnchorRef { get; set; }

        [Networked(OnChanged = nameof(OffsetChanged))]
        private NetworkDictionary<AttachableOffsetKeys, Vector3> Offset => default;

        public void InitForAnchorRef(NetworkId anchorRef, Vector3? offset = null, Vector3? rotationDirection = null)
        {
            AnchorRef = anchorRef;

            if (offset == null || rotationDirection == null)
                return;


            if (offset != null)
                Offset.Add(AttachableOffsetKeys.Position, (Vector3)offset);

            if (rotationDirection != null)
                Offset.Add(AttachableOffsetKeys.RotationDirection, (Vector3)rotationDirection);
        }

        protected static void AnchorRefChanged(Changed<AttachableView> changed)
        {
            changed.Behaviour.Attach(changed.Behaviour.AnchorRef);
        }

        protected static void OffsetChanged(Changed<AttachableView> changed)
        {
            changed.Behaviour.Offset.TryGet(AttachableOffsetKeys.Position, out var position);
            changed.Behaviour.Offset.TryGet(AttachableOffsetKeys.RotationDirection, out var rotationDirection);

            changed.Behaviour.SetOffset(position, rotationDirection);
        }


        private void SetOffset(Vector3 offset, Vector3 rotationDirection)
        {
            var childNta =
                Runner.TryGetNetworkedBehaviourFromNetworkedObjectRef<
                    NetworkTransformAnchor>(Object.Id);

            if (childNta)
            {
                if (offset != default)
                    childNta.transform.position = offset;
                
                if (rotationDirection != default)
                    childNta.transform.rotation = Quaternion.LookRotation(rotationDirection);
            }

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
