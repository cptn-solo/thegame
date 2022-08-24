using Assets.Scripts.Services.Game;
using UnityEngine;

namespace Assets.Scripts.Views.Gen
{
    public class GenBlockView : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private Mesh[] availableMeshes;

        private readonly float defaultLifeTime = .3f;
        private float lifeTime = 0.0f;
        
        public Vector3 WorldAddress { get; set; }
        private GenBlockType blockType;
        public GenBlockType BlockType {
            get => blockType;
            set {
                blockType = value;
                AttachMesh((int)value);
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
                            blockState = value;
                            break;
                        }
                }
            } 
        }        

        private void AttachMesh(int idx) =>
            meshFilter.mesh = availableMeshes[idx];
    }
}