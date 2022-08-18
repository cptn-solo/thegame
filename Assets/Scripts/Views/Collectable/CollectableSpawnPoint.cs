namespace Assets.Scripts.Views
{
    using UnityEngine;

    public sealed class CollectableSpawnPoint : WaypointGizmo
    {
        [SerializeField] private GameObject[] prefabs;

        public GameObject[] Prefabs => prefabs;

        protected override Color Color => Color.yellow;
        protected override float Radius => .5f;
    }

}
