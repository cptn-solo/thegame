using UnityEngine;

namespace Assets.Scripts.Views
{
    public class HealthBarEntityView : MonoBehaviour
    {
        #region SerializeFields
        [SerializeField] private MeshRenderer meshRenderer = null;
        #endregion

        #region Private

        private MaterialPropertyBlock matBlock = null;

        #endregion
        private void Awake()
        {
            matBlock = new MaterialPropertyBlock();
        }

        public Transform GetTransform() => transform;

        public MeshRenderer GetMeshRenderer() => meshRenderer;

    }
}