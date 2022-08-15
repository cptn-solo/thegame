using Assets.Scripts.Data;
using Assets.Scripts.Services.App;
using Example;
using Fusion;
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
                Runner.TryGetInputForPlayer<GameplayInput>(Object.InputAuthority, out var gameplayInput) &&
                gameplayInput.RMB &&
                TryGetCollectableToDrop(out CollectableType collectableType))
            {
                InitSpawnTimer();

                Runner.Spawn(collectablePrefabs[(int)collectableType], transform.position + transform.forward * 2.0f, Quaternion.identity, null, (runner, obj) => InitDroppedCollectable(runner, obj));

                collector.EnqueueForCollection(collectableType, -1);
            }
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

        private void InitDroppedCollectable(NetworkRunner runner, NetworkObject obj)
        {
            List<CollectableSpawnPoint> spawnPoints =
                runner.SimulationUnityScene.GetComponents<CollectableSpawnPoint>();
            var closest = spawnPoints.OrderBy(s => Vector3.Distance(s.transform.position, Object.transform.position)).FirstOrDefault();

            var parentObj = closest.GetComponent<NetworkObject>();
            var attachable = obj.GetComponent<AttachableView>();
            attachable.InitForAnchorRef(parentObj.Id, Object.transform.position + Object.transform.forward * 2, Object.transform.forward);

            var despawnable = obj.GetComponent<Despawnable>();
            despawnable.InitForLifeTime(Random.Range(5.0f, 15.0f));
        }
    }
}