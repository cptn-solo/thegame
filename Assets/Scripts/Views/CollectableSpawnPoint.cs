namespace Assets.Scripts.Views
{
    using UnityEngine;

    public sealed class CollectableSpawnPoint : WaypointGizmo
    {
        [SerializeField] private GameObject prefab;

        public GameObject Prefab => prefab;

        protected override Color Color => Color.yellow;
    }

}
