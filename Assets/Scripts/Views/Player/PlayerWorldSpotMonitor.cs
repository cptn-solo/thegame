using Fusion;
using UnityEngine;
using Zenject;
using Assets.Scripts.Services.Game;
using System.Collections;
using Assets.Scripts.Views;
using Fusion.KCC;
using Example;

namespace Assets.Scripts
{
    public class PlayerWorldSpotMonitor : NetworkBehaviour
    {
        [Inject] private readonly GenBlockSpawnerService terrain;
        
        private bool scanActive;
        private bool scanActiveManual;

        [SerializeField] private LayerMask airBridgeLayer;
        
        private KCC kcc;
        private Vector3 moveDir;

        private void Awake()
        {
            kcc = GetBehaviour<KCC>();
        }

        public void ManualToggleGen(bool toggle)
        {
            scanActiveManual = toggle;
            if (toggle && !scanActive)
                StartCoroutine(ScanNeabyGenBlocks(transform.position.y - transform.localScale.y - 1));
        }
        public override void FixedUpdateNetwork()
        {
            moveDir = kcc.FixedData.InputDirection * (kcc.FixedData.RealSpeed / 2);
        }

        private IEnumerator ScanNeabyGenBlocks(float floorY)
        {
            terrain.StartSpawning(Runner);

            while (scanActive || scanActiveManual)
            {
                terrain.RequestBlocksAround(transform.position + moveDir, Runner, floorY);
                yield return new WaitForSeconds(.2f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CheckColliderMask(other))
            {
                scanActive = true;
                
                if (!scanActiveManual)
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