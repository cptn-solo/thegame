﻿using Fusion;
using Fusion.KCC;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class BallLauncherView : NetworkBehaviour
    {
        [SerializeField] private Transform bullet;
        [SerializeField] private Animator bulletAnimator;
        [SerializeField] private Transform beam;
        [SerializeField] private float beamSpeed = 50.0f;
        [SerializeField] private float beamForceSpeed = .05f;
        [SerializeField] private float beamDistance = 50.0f;
        [SerializeField] private LayerMask beamMask;

        private Vector3 direction;
        private bool shot;
        private Vector3 beamShotPosition = default;
        private Rigidbody rb;
        private readonly float speed = 35.0f;
        private Collider[] beamCollisionsBuffer;

        private void Awake()
        {
            rb = bullet.gameObject.GetComponent<Rigidbody>();
            beamCollisionsBuffer = new Collider[5];
        }
        public void Launch(Vector3 direction, bool secondary = false)
        {
            this.direction = direction;
            shot = false;
            if (secondary)
            {
                beam.SetParent(null);
                beam.localRotation.SetLookRotation(direction);
                beam.gameObject.SetActive(true);
            }
            else 
            {
                bullet.SetParent(null);
                bullet.gameObject.SetActive(true);
            }
        }
        public void Park()
        {
            beam.gameObject.SetActive(false);
            beam.SetParent(transform);
            beam.localPosition = Vector3.zero;
            beam.localRotation = Quaternion.identity;

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
            if (!Runner.IsForward)
                return;

            if (bullet.parent == null)
            {
                if (!shot)
                {
                    rb.AddForce(direction * speed, ForceMode.VelocityChange);
                    shot = true;
                }
                rb.AddForce(direction * 80, ForceMode.Acceleration);
            }

            if (beam.parent == null)
            {
                Vector3 dir = default;
                if (!shot)
                {
                    dir = Vector3.RotateTowards(
                        beam.forward,
                        direction, speed, 0.0f);
                        beam.rotation = Quaternion.LookRotation(dir);
                    beam.position += beam.forward * beamDistance;
                    shot = true;
                    beamShotPosition = beam.position;
                }
                dir = (transform.position - beam.position).normalized;
                beam.rotation = Quaternion.LookRotation(dir);
                var distance = Vector3.Distance(beam.position, transform.position);
                if (distance < 5.0f)
                    beam.position = beamShotPosition;
                
                beam.position += beamSpeed * Runner.DeltaTime * beam.forward;                
                beam.RotateAround(beam.position, beam.forward, distance);
                var collectableHits = Runner.GetPhysicsScene().OverlapSphere(
                    beam.position, 2.0f, beamCollisionsBuffer, beamMask, QueryTriggerInteraction.UseGlobal);
                if (collectableHits > 0)
                {
                    for (int i = 0; i < collectableHits; i++)
                    {
                        var collectableRb = beamCollisionsBuffer[i].gameObject.GetComponent<Rigidbody>();
                        collectableRb.AddForce(
                            (transform.root.position - collectableRb.position).normalized * beamForceSpeed, 
                            ForceMode.VelocityChange);
                    }

                    beamCollisionsBuffer.Clear();
                }
            }

            //bullet.position += Runner.DeltaTime * speed * direction;

        }
    }
}