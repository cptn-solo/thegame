using Fusion;
using UnityEngine;
using Zenject;
using Assets.Scripts.Services.Game;
using System.Collections;
using Assets.Scripts.Views;

namespace Assets.Scripts
{
    public class PlayerWorldSpotMonitor : NetworkBehaviour
    {
        [Inject] private readonly GenBlockSpawnerService terrain;
        
        private bool scanActive;
        private bool scanActiveManual;

        [SerializeField] private LayerMask airBridgeLayer;
        
        public void ManualToggleGen(bool toggle)
        {
            scanActiveManual = toggle;
            if (toggle)
                StartCoroutine(ScanNeabyGenBlocks(transform.position.y - transform.localScale.y - 1));
        }

        private IEnumerator ScanNeabyGenBlocks(float floorY)
        {
            terrain.StartSpawning(Runner);

            while (scanActive || scanActiveManual)
            {
                terrain.RequestBlocksAround(transform.position, Runner, floorY);
                yield return new WaitForSeconds(.2f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CheckColliderMask(other))
            {
                scanActive = true;
                StartCoroutine(ScanNeabyGenBlocks(transform.position.y - transform.localScale.y - 1));
            }
        }

        private bool CheckColliderMask(Collider other)
        {
            return (airBridgeLayer.value & (1 << other.transform.gameObject.layer)) > 0;
        }

        private void OnTriggerStay(Collider other)
        {
            if (CheckColliderMask(other))
                scanActive = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (CheckColliderMask(other))
                scanActive = false;
        }
    }
}