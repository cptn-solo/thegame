using Assets.Scripts.Views;
using Example;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services.Game
{
    public class ArtefactSpawnerService : MonoBehaviour
    {
        private List<NetworkObject> spawned = new List<NetworkObject>();

        private bool spawning;

        private readonly float cooldown;

        internal void StartSpawning(NetworkRunner runner)
        {
            spawning = true;
            
            if (runner.IsServer)
                StartCoroutine(nameof(SpawnArtefactsCoroutine), runner);
        }

        internal void StopSpawning(NetworkRunner runner)
        {
            spawning = false;
            foreach (var item in spawned)
                runner.Despawn(item);
        }

        private IEnumerator SpawnArtefactsCoroutine(NetworkRunner runner)
        {
            List<CollectableSpawnPoint> spawnPoints = runner.SimulationUnityScene.GetComponents<CollectableSpawnPoint>();
            while (spawning)
            {
                foreach (var point in spawnPoints)
                {
                    var prefab = point.Prefabs[Random.Range(0, point.Prefabs.Length)];

                    runner.Spawn(prefab, Vector3.zero, Quaternion.identity, null,
                        (runner, obj) => InitCollectable(runner, obj, point.GetComponent<NetworkObject>()));

                    yield return new WaitForSeconds(Random.Range(1.0f, 5.0f));
                }
            }
        }

        public static Vector3 RndDirection() =>
            new(
                Random.Range(-1.0f, 1.0f),
                Random.Range(0.0f, 0.3f),
                Random.Range(-1.0f, 1.0f));

        private void InitCollectable(NetworkRunner runner, NetworkObject obj, NetworkObject parentObj)
        {

            var position = parentObj.transform.position;
            var offset = RndDirection() * Random.Range(1, 5);
            var moveDirectioin = RndDirection();

            var attachable = obj.GetComponent<AttachableView>();
            attachable.InitForAnchorRef(parentObj.Id, position + offset, moveDirectioin);
            
            var movable = obj.GetComponent<MovableView>();
            movable.speed = moveDirectioin * Random.Range(.2f, 2.0f);
            
            var despawnable = obj.GetComponent<Despawnable>();
            despawnable.InitForLifeTime(Random.Range(15.0f, 35.0f));
        }


        private IEnumerator DespawnArtefactsCoroutine(NetworkRunner runner)
        {
            var enumerator = spawned.GetEnumerator();
            enumerator.MoveNext();
            var item = enumerator.Current;
            while (item != null)
                yield return null;

            if (runner != null && item != null)
                runner.Despawn(item);
        }
    }
}