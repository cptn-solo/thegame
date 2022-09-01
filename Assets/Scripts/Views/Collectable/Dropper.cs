using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Assets.Scripts.Services.Game;
using Example;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class Dropper : NetworkBehaviour
    {
        [Inject] private readonly PlayerInventoryService playerInventory;


        [SerializeField] private GameObject[] collectablePrefabs;
        [SerializeField] private LayerMask hitLayerMask;

        private bool hitDetected;

        [Networked] private TickTimer spawnTimer { get; set; }

        private Collector collector;
        private PlayerInput input;
        private readonly float coolDownTime = 1.0f;

        private void Awake()
        {
            collector = GetComponent<Collector>();
            input = GetComponent<PlayerInput>();
        }

        private void InitSpawnTimer()
        {
            spawnTimer = TickTimer.CreateFromSeconds(Runner, coolDownTime);
        }
        private NetworkObjectPredictionKey PredictionKey()
        {
            var predictionKey = new NetworkObjectPredictionKey
            {
                Byte0 = (byte)Runner.Simulation.Tick,
                Byte1 = (byte)Object.InputAuthority.PlayerId
            };
            return predictionKey;
        }
        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer &&
                spawnTimer.ExpiredOrNotRunning(Runner) &&
                ((Runner.TryGetInputForPlayer<GameplayInput>(Object.InputAuthority, out var gameplayInput) &&
                gameplayInput.RMB) || hitDetected))
            {
                InitSpawnTimer();

                StartCoroutine(nameof(DropItems), hitDetected);
            }
        }

        private IEnumerator DropItems(bool wasHit = false)
        {
            var dropCount = collector.GetRandomDropCount();

            if (!wasHit && dropCount > 0)
                dropCount = 1;

            yield return null;

            for (var i = 0; i < dropCount; i++)
            {
                if (!collector.TryGetCollectableToDrop(out CollectableType collectableType))
                    break;

                var dropDirection = wasHit ?
                    ArtefactSpawnerService.RndDirection() :
                    transform.forward;
                var dropPosition = wasHit ?
                    transform.position + 2.0f * transform.localScale.y * dropDirection :
                    transform.position + 1.0f * transform.localScale.y * transform.forward;

                yield return null;

                Runner.Spawn(collectablePrefabs[(int)collectableType], dropPosition, Quaternion.identity, null, (runner, obj) => InitDroppedCollectable(runner, obj, dropPosition, dropDirection));

                collector.EnqueueForCollection(collectableType, -1);
                yield return null;
            }

            hitDetected = false;
        }

        private void InitDroppedCollectable(NetworkRunner runner, NetworkObject obj, Vector3 dropPosition, Vector3 dropDirection)
        {
            List<CollectableSpawnPoint> spawnPoints =
                runner.SimulationUnityScene.GetComponents<CollectableSpawnPoint>();
            var closest = spawnPoints.OrderBy(s => Vector3.Distance(s.transform.position, Object.transform.position)).FirstOrDefault();

            var parentObj = closest.GetComponent<NetworkObject>();
            var attachable = obj.GetComponent<AttachableView>();
            attachable.InitForAnchorRef(parentObj.Id, dropPosition, Object.transform.forward);

            var movable = obj.GetComponent<MovableView>();
            movable.speed = dropDirection * Random.Range(4f, 5f);
            movable.mass = .1f;
            movable.useGravity = true;

            var despawnable = obj.GetComponent<Despawnable>();
            despawnable.InitForLifeTime(Random.Range(5.0f, 15.0f));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CheckColliderMask(hitLayerMask) && other.gameObject.activeSelf)
            {
                hitDetected = true;
                Debug.Log($"Hit with layer {other.gameObject.layer}");

            }
        }
    }
}