using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Assets.Scripts.Services.Game;
using Example;
using Fusion;
using Fusion.KCC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Views
{
    public class Dropper : NetworkBehaviour
    {
        [Inject] private readonly PlayerInventoryService playerInventory;


        [SerializeField] private GameObject[] collectablePrefabs;

        [Networked] private TickTimer spawnTimer { get; set; }

        private Collector collector;
        private PlayerInput input;
        private KCC kcc;
        private readonly float coolDownTime = 1.0f;

        private bool hitDetected;

        private void Awake()
        {
            collector = GetComponent<Collector>();
            input = GetComponent<PlayerInput>();
            kcc = GetComponent<KCC>();
            
            kcc.OnCollisionEnter += Kcc_OnCollisionEnter;
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

                DropItems(hitDetected);
            }
        }

        private void DropItems(bool wasHit = false)
        {
            var dropCount = GetRandomDropCount();

            if (!wasHit && dropCount > 0)
                dropCount = 1;

            for (var i = 0; i < dropCount; i++)
            {
                if (!TryGetCollectableToDrop(out CollectableType collectableType))
                    break;

                var dropPosition = wasHit ?  
                GetRandomDropPosition() :
                transform.position + transform.forward * 2.0f;
                
                Runner.Spawn(collectablePrefabs[(int)collectableType], dropPosition, Quaternion.identity, null, (runner, obj) => InitDroppedCollectable(runner, obj, dropPosition));

                collector.EnqueueForCollection(collectableType, -1);
            }

            hitDetected = false;
        }

        private int GetRandomDropCount()
        {
            var totalCount = collector.Collected.Where(c => c.Value > 0).Select(c => c.Value).Sum();
            if (totalCount == 0)
                return 0;
                
            return Random.Range(1, Mathf.FloorToInt(Mathf.Sqrt(totalCount)));
        }

        private Vector3 GetRandomDropPosition()
        {
            var position = transform.position;
            var offset = ArtefactSpawnerService.RndDirection() * Random.Range(2, 5);
            return position + offset;
        }

        private bool TryGetCollectableToDrop(out CollectableType collectableType)
        {
            collectableType = default;

            var available = collector.Collected.Where(c => c.Value > 0).Select(c => c.Key).ToArray();
            if (available.Length > 0)
            {
                collectableType = available[Random.Range(0, available.Length)];
                return true;
            }

            return false;
        }

        private void InitDroppedCollectable(NetworkRunner runner, NetworkObject obj, Vector3 dropPosition)
        {
            List<CollectableSpawnPoint> spawnPoints =
                runner.SimulationUnityScene.GetComponents<CollectableSpawnPoint>();
            var closest = spawnPoints.OrderBy(s => Vector3.Distance(s.transform.position, Object.transform.position)).FirstOrDefault();

            var parentObj = closest.GetComponent<NetworkObject>();
            var attachable = obj.GetComponent<AttachableView>();
            attachable.InitForAnchorRef(parentObj.Id, dropPosition, Object.transform.forward);

            var despawnable = obj.GetComponent<Despawnable>();
            despawnable.InitForLifeTime(Random.Range(5.0f, 15.0f));
        }


        private void OnDestroy()
        {
            kcc.OnCollisionEnter -= Kcc_OnCollisionEnter;
        }

        private void Kcc_OnCollisionEnter(KCC arg1, KCCCollision arg2)
        {
            if (arg2.NetworkObject.TryGetBehaviour<ShellView>(out var shell))
                hitDetected = true;
        }
    }
}