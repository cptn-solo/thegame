using Assets.Scripts.Views.Gen;
using Fusion;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Services.Game
{
    public enum GenBlockType
    {
        None = 0,
        Generic = 1
    }

    public enum GenBlockStage
    {
        Stale = 0,
        Requested = 1,
        Deleted = 2,
    }
    public class GenBlockSpawnerService : MonoBehaviour
    {
        [SerializeField] private Mesh sampleBlockMesh;
        [SerializeField] private int spotSize = 1; // -2 -1 0 1 2 , so 5x5 spot in the world is covered
        [SerializeField] private float worldY = -50.0f;

        [SerializeField] private GameObject genBlockPrefab;

        private float meshSide;
        private float meshHeight;

        private NetworkRunner runner;
        private Dictionary<Vector3, GenBlockView> requestedBlocks = new();

        public void StartSpawning(NetworkRunner runner)
        {
            if (this.runner && this.runner.Equals(runner))
                return;

            this.runner = runner;

            meshSide = sampleBlockMesh.bounds.size.x;
            meshHeight = sampleBlockMesh.bounds.size.y;
        }

        public void StopSpawning(NetworkRunner runner)
        {
        }

        public void RequestBlocksAround(Vector3 pos, NetworkRunner runner, float floorY = default)
        {
            var floorYbuff = worldY;// floorY == default ? pos.y - 2 : floorY;

            Debug.Log($"RequestBlocksAround {pos} runner {runner.LocalPlayer}");
            var globalXOffset = pos.x % meshSide;
            var globalZOffset = pos.z % meshSide;
            var globalYOffset = floorYbuff % meshHeight;

            var globalX = (pos.x - globalXOffset) / meshSide;
            var globalZ = (pos.z - globalZOffset) / meshSide;
            var globalY = (floorYbuff - globalYOffset) / meshHeight;

            var buff = Vector3.zero;
            var globalBuff = Vector3.zero;

            foreach (var block in requestedBlocks.Values)
                block.BlockState = GenBlockStage.Stale;

            for (int x = -1 * spotSize; x <= 1 * spotSize; x++)
                for (int z = -1 * spotSize; z <= 1 * spotSize; z++)
                {
                    buff.x = globalX + x;
                    buff.z = globalZ + z;
                    buff.y = globalY;

                    if (requestedBlocks.TryGetValue(buff, out var block))
                    {
                        block.BlockState = GenBlockStage.Requested;
                    }
                    else
                    {
                        globalBuff.x = buff.x * meshSide;
                        globalBuff.z = buff.z * meshSide;
                        globalBuff.y = floorYbuff;

                        var obj = Instantiate(genBlockPrefab, globalBuff, Quaternion.identity);
                        //runner.Spawn(genBlockPrefab, globalBuff);
                        block = obj.GetComponent<GenBlockView>();
                        block.BlockType = GenBlockType.Generic;
                        block.WorldAddress = globalBuff;
                        block.BlockState = GenBlockStage.Requested;

                        requestedBlocks.Add(buff, block);
                    }
                }

            List<Vector3> deleted = new();
            foreach (var key in requestedBlocks.Keys)
            {
                var block = requestedBlocks[key];
                if (block.BlockState == GenBlockStage.Deleted)
                {
                    if (!block.gameObject.IsDestroyed())
                        Object.Destroy(block.gameObject);

                    deleted.Add(key);
                }
            }

            foreach (var key in deleted)
                requestedBlocks.Remove(key);
        }
    }
}