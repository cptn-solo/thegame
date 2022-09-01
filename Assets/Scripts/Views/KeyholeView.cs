using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class KeyholeView : NetworkBehaviour
    {
        private const string AnimatorReadyBoolKey = "ready";
        private const string AnimatorEngageBoolKey = "engage";

        private Animator animator;

        [SerializeField] private LayerMask playerMask;

        private MovingIsland movingIsland;

        [Networked(OnChanged = nameof(OnReadyChanged))]
        public NetworkBool ready { get; set; }

        [Networked(OnChanged = nameof(OnActivatedChanged))]
        public NetworkBool activated { get; set; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            movingIsland = GetComponentInParent<MovingIsland>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(playerMask) &&
                other.TryGetComponent<Collector>(out var collector) &&
                collector.TryGetCollectableTypeToDrop(movingIsland.KeyCollectableType, true))
            {
                activated = true;
            }
        }

        private static void OnReadyChanged(Changed<KeyholeView> changed)
        {
            var current = changed.Behaviour.ready;
            changed.LoadOld();
            var prev = changed.Behaviour.ready;

            if (current != prev)
                changed.Behaviour.SetLocalReadyState(current);
        }

        private static void OnActivatedChanged(Changed<KeyholeView> changed)
        {
            var current = changed.Behaviour.activated;
            changed.LoadOld();
            var prev = changed.Behaviour.activated;

            if (current != prev)
                changed.Behaviour.SetLocalActivatedState(current);
        }

        public override void FixedUpdateNetwork()
        {

        }

        internal void SetReadyState(bool ready) =>
            this.ready = ready;

        private void SetLocalActivatedState(bool activated)
        {
            animator.SetBool(AnimatorEngageBoolKey, activated);

            if (activated)
                movingIsland.OnKeyholeActivated(this);
        }

        private void SetLocalReadyState(bool ready) =>
            animator.SetBool(AnimatorReadyBoolKey, ready);
    }
}