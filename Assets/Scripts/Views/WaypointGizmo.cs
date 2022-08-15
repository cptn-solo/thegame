using UnityEngine;

namespace Assets.Scripts.Views
{
    public class WaypointGizmo : MonoBehaviour
    {

        protected virtual Color Color { get => Color.blue; }
        protected virtual float Radius { get => 5; }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            Gizmos.color = Color;
            Gizmos.DrawSphere(transform.position, Radius);
            #endif
        }
    }
}