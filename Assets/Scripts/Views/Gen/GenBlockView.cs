using Assets.Scripts.Services.Game;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views.Gen
{
    public class GenBlockView : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private Mesh[] availableMeshes;

        private float defaultLifeTime = .3f;
        private float lifeTime = 0.0f;
        
        public Vector3 WorldAddress { get; set; }
        private GenBlockType blockType;
        public GenBlockType BlockType {
            get => blockType;
            set {
                blockType = value;
                AttachMesh(Random.Range(0, availableMeshes.Length));
            } }

        private GenBlockStage blockState;
        public GenBlockStage BlockState {
            get => blockState;
            set {
                switch (value)
                {
                    case GenBlockStage.Requested:
                        {
                            blockState = value;
                            lifeTime = defaultLifeTime;
                            break;
                        }
                    case GenBlockStage.Stale:
                        {
                            blockState = value;
                            lifeTime -= .1f;
                            if (lifeTime < 0)
                                blockState = GenBlockStage.Deleted;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            } }
                
        private void Start()
        {
            // demo mode, remove when not needed
            //StartCoroutine(SwitchMesh());
        }     
        
        private IEnumerator SwitchMesh()
        {
            while (true)
            {
                AttachMesh(Random.Range(0, availableMeshes.Length));

                //for (var idx = 0; idx < availableMeshes.Length; idx++)
                //{
                //    AttachMesh(idx);

                //    yield return new WaitForSeconds(2.0f);
                //}
                yield return new WaitForSeconds(2.0f);
            }
        }

        private void AttachMesh(int idx)
        {
            var mesh = availableMeshes[idx];
            //mesh.bounds = new Bounds(Vector3.zero, Vector3.zero);
            //mesh.RecalculateBounds();
            meshFilter.mesh = mesh;
        }
    }
}