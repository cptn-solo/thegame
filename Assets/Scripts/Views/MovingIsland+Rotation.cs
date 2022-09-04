using Assets.Scripts.Data;
using Fusion;
using Fusion.KCC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Views
{
    internal struct FlipEvent : INetworkStruct
    {
        public AxisMapped nextRotationAxis;
        public int count;
    }
    internal enum AxisMapped
    {
        NA = 0,
        x = 1,
        z = 2
    }

    public sealed partial class MovingIsland
    {
        [SerializeField] private NetworkTransform visualsTransform;
        [SerializeField] private CollectableType keyCollectableType = CollectableType.Key0;

        public CollectableType KeyCollectableType => keyCollectableType;

        [Networked(OnChanged = nameof(OnNextRotationChange))]
        private FlipEvent nextRotationAxis { get; set; }

        [Networked(OnChanged = nameof(OnAllSidesUnlockedChange))]
        private NetworkBool allSidesUnlocked { get; set; }
        
        private bool rotating = false;
        private bool keyHoleRotating = false;

        [Networked]
        private NetworkBool flipActivated { get; set; }

        private bool prevFlipActivated;

        private List<AxisMapped> sidesSequence = new()
        {
            AxisMapped.x,
            AxisMapped.z,
            AxisMapped.x,
            AxisMapped.z,
            AxisMapped.x, 
            AxisMapped.z // last returns the isle to its initial rotation
        };

        private KeyholeView[] keyHoles;

        [SerializeField] private float hopImpulse = 60.0f;

        private static void OnNextRotationChange(Changed<MovingIsland> changed)
        {
            var nextRotation = changed.Behaviour.nextRotationAxis;

            changed.LoadOld();

            var nextRotationOld = changed.Behaviour.nextRotationAxis;

            if (nextRotation.count != nextRotationOld.count)
                changed.Behaviour.EngageFlip(nextRotation.nextRotationAxis);

        }

        private static void OnAllSidesUnlockedChange(Changed<MovingIsland> changed)
        {
            var current = changed.Behaviour.allSidesUnlocked;

            if (current)
                changed.Behaviour.OnAllSidesUnlocked();

        }

        private void OnAllSidesUnlocked()
        {
            Debug.Log($"MovingIsland complete");
            //TODO: next level if Battle Royal mode is on
        }

        public void OnKeyholeActivated(KeyholeView keyhole)
        {
            var readyKeyholes = keyHoles.Where(x => x.ready).ToArray();
            var activatedKeyholes = keyHoles.Where(x => x.activated).ToArray();
            
            if (readyKeyholes.Length > 0 && activatedKeyholes.Length == readyKeyholes.Length &&
                activatedKeyholes.Length < keyHoles.Length)
                EnqueueRotation();
        }

        private void IsleFlipperOnAwake()
        {
            keyHoles = GetComponentsInChildren<KeyholeView>();
        }

        private void IsleFlipperOnFUN()
        {
            if (Runner.IsServer && visualsTransform != null)
            {
                if (!rotating && keyHoles.Length == 0) // only autorotate if no keyholes to activate
                {
                    rotating = true;
                    StartCoroutine(ScheduleVisualFlip());
                }

                if (!keyHoleRotating && keyHoles.Length > 0)
                {
                    keyHoleRotating = true;
                    StartCoroutine(ScheduleNextKeyHolesReady());
                }
            }
        }

        private void IsleFlipperKCCOnStay(KCC kcc)
        {
            if (flipActivated &&
                (Runner.IsServer || Runner.IsResimulation) 
                && kcc.FixedData.ExternalImpulse == default)
                kcc.AddExternalImpulse(Vector3.up * hopImpulse);
        }

        private IEnumerator ScheduleVisualFlip()
        {
            while (rotating)
            {
                yield return new WaitForSeconds(Random.Range(15.0f, 25.0f));

                EnqueueRotation();
            }
        }

        private void EnqueueRotation()
        {
            // x, z, x, z, x, z - gives all 6 sides covered
            var x = nextRotationAxis;
            x.nextRotationAxis = keyHoles.Length > 0 ? 
                sidesSequence[x.count] : 
                (AxisMapped)Random.Range(1, 3);
            if (x.count < sidesSequence.Count - 1)
            {
                x.count++;
            }
            else
            {
                x.count = 0;
                allSidesUnlocked = true;
            }
            nextRotationAxis = x;
        }

        private void EngageFlip(AxisMapped nextRotationAxis)
        {
            if (!Runner.IsServer)
                return;

            StartCoroutine(Rotate90(nextRotationAxis));
            StartCoroutine(ScheduleNextKeyHolesReady());
        }

        IEnumerator Rotate90(AxisMapped nextRotationAxis)
        {
            yield return new WaitForSeconds(5.0f);

            flipActivated = true;
            
            yield return new WaitForSeconds(1.5f);

            var axis = nextRotationAxis == AxisMapped.x ? Vector3.right : Vector3.forward;
            var duration = 3.0f;
            var totalAngle = 90.0f;
            
            var rotationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, duration, totalAngle);
            rotationCurve.keys[0].outTangent = .0f;
            rotationCurve.keys[0].weightedMode = WeightedMode.Out;
            rotationCurve.keys[0].outWeight = .2f;
            rotationCurve.keys[1].inTangent = .5f;
            rotationCurve.keys[1].weightedMode = WeightedMode.In;
            rotationCurve.keys[1].inWeight = .2f;

            var percent = 0.0f;
            var angle = 0.0f;
            
            while (percent <= duration)
            {
                percent += Runner.DeltaTime;
                var delta = rotationCurve.Evaluate(percent) - angle;
                visualsTransform.Transform.RotateAround(visualsTransform.Transform.position, axis, delta);
                angle += delta;

                yield return null;
            }
            // the remainder
            Debug.Log($"Rotated {angle}");

            visualsTransform.Transform.RotateAround(visualsTransform.Transform.position, axis, totalAngle - angle);

            flipActivated = false;
        }

        private IEnumerator ScheduleNextKeyHolesReady()
        {
            yield return new WaitForSeconds(15.0f);

            var upperKeyHoles = keyHoles
                .Where(x => !x.activated && !x.ready &&
                    Vector3.Angle(x.transform.up, Vector3.up) < .2f)
                .ToArray();

            foreach(var kh in upperKeyHoles)
                kh.SetReadyState(true);
        }
    }
}