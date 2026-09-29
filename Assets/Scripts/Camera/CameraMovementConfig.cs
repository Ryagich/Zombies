using UnityEngine;

namespace Zombies.Input
{
    [CreateAssetMenu(fileName = "CameraMovementConfig", menuName = "Zombies/Camera Movement Config")]
    public sealed class CameraMovementConfig : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float ScreenEdgeSize { get; private set; } = 24f;
        [field: SerializeField, Min(0f)] public float EdgeMoveSpeed { get; private set; } = 12f;
        [field: SerializeField, Min(0f)] public float DragMoveSensitivity { get; private set; } = 0.025f;
        [field: SerializeField] public bool InvertDrag { get; private set; } = true;

    }
}
