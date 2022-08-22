using Fusion;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class BallLauncherView : NetworkBehaviour
    {
        [SerializeField] private Transform bullet;
        [SerializeField] private Animator bulletAnimator;
        private Vector3 direction;
        private bool shot;
        private Rigidbody rb;
        private readonly float speed = 35.0f;

        private void Awake()
        {
            rb = bullet.gameObject.GetComponent<Rigidbody>();
        }
        public void Launch(Vector3 direction)
        {
            this.direction = direction;
            shot = false;
            bullet.SetParent(null);
            bullet.gameObject.SetActive(true);
        }
        public void Park()
        {
            bullet.gameObject.SetActive(false);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.rotation = Quaternion.identity;
            bullet.SetParent(transform);
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
        }
        public override void FixedUpdateNetwork()
        {
            if (Runner.IsForward && bullet.parent == null)
            {
                if (!shot)
                {
                    rb.AddForce(direction * speed, ForceMode.VelocityChange);
                    shot = true;
                }
                rb.AddForce(direction * 80, ForceMode.Acceleration);
            }
            //bullet.position += Runner.DeltaTime * speed * direction;

        }
    }
}