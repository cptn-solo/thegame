using UnityEngine;

namespace Assets.Scripts.Views
{
    public class WaypointGizmo : MonoBehaviour
    {
        // Start is called before the first frame update
        private void OnDrawGizmos()
        {

            #if UNITY_EDITOR
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 1);
            #endif
        }
    }
}