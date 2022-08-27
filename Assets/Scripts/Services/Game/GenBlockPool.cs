using Assets.Scripts.Views.Gen;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Services.Game
{
    public class GenBlockPool : MonoBehaviour
    {
        [Inject] private readonly GenBlockSpawnerService terrainSpawner;

        private const int blockTypesCount = 10; // for GenBlockType
        private const int blockTypePoolSize = 10;

        private readonly Dictionary<GenBlockType, int> spareBlockCount = new(blockTypesCount);
        private readonly Dictionary<GenBlockType, int> lastReleasedIdx = new(blockTypesCount);
        private readonly Dictionary<GenBlockType, List<GenBlockView>> pool = new(blockTypesCount);

        [SerializeField] private GameObject genBlockPrefab;

        private void Start()
        {
            StartCoroutine(WarmupPool());
        }

        private IEnumerator WarmupPool()
        {
            for (var i = 0; i < blockTypesCount; i++)
            {
                var blockType = (GenBlockType)i;
                pool[blockType] = new(blockTypePoolSize);

                ExtendBlockTypePool(blockType);

                lastReleasedIdx[(GenBlockType)i] = 0;
                yield return null;
            }

            terrainSpawner.GenBlockPool = this;
        }

        private void ExtendBlockTypePool(GenBlockType blockType)
        {
            var typeList = pool[blockType];

            for (var j = 0; j < blockTypePoolSize; j++)
            {
                var block = Instantiate(genBlockPrefab, Vector3.zero, Quaternion.identity)
                    .GetComponent<GenBlockView>();
                block.transform.parent = transform;
                typeList.Add(block);
                block.BlockType = blockType;
                block.gameObject.SetActive(false);
            }
            if (spareBlockCount.TryGetValue(blockType, out var cnt))
                spareBlockCount[blockType] = cnt + blockTypePoolSize;
            else spareBlockCount[blockType] = blockTypePoolSize;

            Debug.Log($"Gen block pool extended by {blockTypePoolSize} for block type {blockType}. " +
                $"New size is {typeList.Count}, spare count is {spareBlockCount[blockType]}");

        }

        private GenBlockView GetSpareBlock(GenBlockType blockType)
        {
            var cnt = spareBlockCount[blockType];
            spareBlockCount[blockType]--;
            GenBlockView spare = null;
            if (cnt > 0)
            {
                if (lastReleasedIdx[blockType] >= 0)
                {
                    spare = pool[blockType][lastReleasedIdx[blockType]];
                    lastReleasedIdx[blockType] = -1;
                }
                else
                {
                    spare = pool[blockType].First(x => x.gameObject.activeSelf == false);
                }
            }
            else
            {
                ExtendBlockTypePool(blockType);

                return GetSpareBlock(blockType);
            }

            return spare;
        }
        public GenBlockView RequestBlockForWorldPosition(Vector3 globalBuff, Vector3 worldAddress)
        {
            var block = GetSpareBlock(GenBlockType.Generic1);

            block.WorldAddress = worldAddress;
            block.BlockState = GenBlockStage.Requested;
            block.transform.SetPositionAndRotation(globalBuff, Quaternion.identity);

            block.gameObject.SetActive(true);

            return block;
        }

        public void ReleaseBlock(GenBlockView block)
        {
            block.gameObject.SetActive(false);

            block.WorldAddress = Vector3.zero;
            block.BlockState = GenBlockStage.Deleted;
            block.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            lastReleasedIdx[block.BlockType] = pool[block.BlockType].IndexOf(block);
            spareBlockCount[block.BlockType]++;
        }
    }
}