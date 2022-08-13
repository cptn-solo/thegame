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

        private float timer;
        private readonly float cooldown;

        internal void StartSpawning(NetworkRunner runner)
        {
            spawning = true;
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
                    var position = point.transform.position;
                    var offset = new Vector3(
                        Random.Range(-1.0f, 1.0f),
                        Random.Range(-1.0f, 1.0f),
                        0) * Random.Range(1, 5);

                    runner.Spawn(prefab, position + offset, point.transform.rotation, null,
                        (runner, obj) => InitCollectable(runner, obj, point.GetComponent<NetworkObject>()));

                    yield return new WaitForSeconds(Random.Range(.1f, 5.0f));
                }
            }
        }

        private void InitCollectable(NetworkRunner runner, NetworkObject obj, NetworkObject parentObj)
        {
            var attachable = obj.GetComponent<AttachableView>();
            attachable.InitForAnchorRef(parentObj.Id);
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