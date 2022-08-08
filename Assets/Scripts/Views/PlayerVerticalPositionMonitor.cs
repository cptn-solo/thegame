using Example;
using Fusion;
using Fusion.KCC;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerVerticalPositionMonitor : NetworkBehaviour
    {
        [SerializeField] private float fallFromIslandY = -100.0f;
        private KCC kcc;

        private void Awake()
        {
            kcc = GetBehaviour<KCC>();
        }
        public override void FixedUpdateNetwork()
        {
            if (transform.position.y < fallFromIslandY)
            {
                List<SpawnPoint> spawnPoints = Runner.SimulationUnityScene.GetComponents<SpawnPoint>();
                Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].transform;

                transform.position = spawnPoint.position;

                kcc.SetPosition(spawnPoint.position);

                kcc.SetLookRotation(spawnPoint.rotation, true);
                kcc.SetDynamicVelocity(Vector3.zero);
                kcc.SetKinematicVelocity(Vector3.zero);

            }
        }
    }
}