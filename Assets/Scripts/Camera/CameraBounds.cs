using UnityEngine;

namespace Zombies.CameraSystem
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public sealed class CameraBounds : MonoBehaviour
    {
        [SerializeField] private BoxCollider boundsCollider;

        public Bounds WorldBounds => boundsCollider.bounds;

        private void Reset() => boundsCollider = GetComponent<BoxCollider>();

        private void OnValidate()
        {
            if (boundsCollider == null)
                boundsCollider = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmosSelected()
        {
            if (boundsCollider == null)
                return;

            Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawCube(boundsCollider.bounds.center, boundsCollider.bounds.size);
            Gizmos.color = new Color(0.2f, 0.8f, 1f, 1f);
            Gizmos.DrawWireCube(boundsCollider.bounds.center, boundsCollider.bounds.size);
        }
    }
}
