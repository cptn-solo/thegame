using Fusion;
using System;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class BallLauncherView : NetworkBehaviour
    {
        [SerializeField] private Transform bullet;
        [SerializeField] private Animator bulletAnimator;
        private Vector3 direction;
        private readonly float speed = 10.0f;

        public void Launch(Vector3 direction)
        {
            this.direction = direction;
            bullet.gameObject.SetActive(true);
            bullet.SetParent(null);
        }
        public void Park()
        {
            bullet.SetParent(transform);
            bullet.localPosition = Vector3.zero;
            bullet.gameObject.SetActive(false);
        }
        public override void FixedUpdateNetwork()
        {
            if (Runner.IsForward && bullet.parent == null)
                bullet.position += Runner.DeltaTime * speed * direction;
        }
    }
}