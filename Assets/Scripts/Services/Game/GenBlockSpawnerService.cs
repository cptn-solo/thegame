using Assets.Scripts.Views.Gen;
using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Services.Game
{

    public class GenBlockSpawnerService : MonoBehaviour
    {
        [SerializeField] private Mesh sampleBlockMesh;
        [SerializeField] private int spotSize = 1; // -2 -1 0 1 2 , so 5x5 spot in the world is covered
        [SerializeField] private float worldY = -50.0f;
        
        private float meshSide;
        private float meshHeight;
        private float floorHeight;

        private NetworkRunner runner;
        private Dictionary<Vector3, GenBlockView> requestedBlocks = new(50);
        private List<Vector3> deleted = new(50);

        public GenBlockPool GenBlockPool { get; set; }

        public void StartSpawning(NetworkRunner runner)
        {
            if (this.runner && this.runner.Equals(runner))
                return;

            this.runner = runner;

            meshSide = sampleBlockMesh.bounds.size.x;
            meshHeight = sampleBlockMesh.bounds.size.y;
            floorHeight = 15.0f;
        }

        public void StopSpawning(NetworkRunner runner)
        {
        }

        public void RequestBlocksAround(Vector3 pos, NetworkRunner runner, float floorY)
        {
            if (GenBlockPool == null)
                return; // still warming up the pool

            //var floorYbuff = worldY;
            var floorYbuff = floorY;

            var globalXOffset = pos.x % meshSide;
            var globalZOffset = pos.z % meshSide;
            var globalYOffset = floorYbuff % floorHeight;

            var globalX = (pos.x - globalXOffset) / meshSide;
            var globalZ = (pos.z - globalZOffset) / meshSide;
            var globalY = (floorYbuff - globalYOffset) / floorHeight;

            var buff = Vector3.zero;
            var globalBuff = Vector3.zero;

            foreach (var block in requestedBlocks.Values)
                block.BlockState = GenBlockStage.Stale;

            for (int x = -1 * spotSize; x <= 1 * spotSize; x++)
                for (int z = -1 * spotSize; z <= 1 * spotSize; z++)
                {
                    buff.x = globalX + x;
                    buff.z = globalZ + z;
                    buff.y = globalY - 1; //the flow below the player

                    if (requestedBlocks.TryGetValue(buff, out var block))
                    {
                        block.BlockState = GenBlockStage.Requested;
                    }
                    else
                    {
                        globalBuff.x = buff.x * meshSide;
                        globalBuff.z = buff.z * meshSide;
                        globalBuff.y = buff.y * floorHeight;

                        //runner.Spawn(genBlockPrefab, globalBuff);
                        block = GenBlockPool.RequestBlockForWorldPosition(globalBuff, buff);

                        requestedBlocks.Add(buff, block);
                    }
                }

            ReleaseStaleBlocks();
        }
        
        

        private void ReleaseStaleBlocks()
        {
            deleted.Clear();
            
            foreach (var key in requestedBlocks.Keys)
            {
                var block = requestedBlocks[key];
                if (block.BlockState == GenBlockStage.Deleted)
                {
                    GenBlockPool.ReleaseBlock(block);
                    deleted.Add(key);
                }
            }

            foreach (var key in deleted)
                requestedBlocks.Remove(key);

        }
    }
}