using UnityEngine;
using UnityEngine.Serialization;

namespace Zombies.NPC
{
    [CreateAssetMenu(fileName = "VisionConfig", menuName = "Configs/NPC/Vision Config")]
    public sealed class VisionConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float ViewDistance { get; private set; } = 8f;
        [field: SerializeField, Range(0f, 360f)] public float ViewAngle { get; private set; } = 90f;
        [field: SerializeField, Min(0f)] public float AttackViewDistance { get; private set; } = 2f;
        [field: SerializeField, Range(0f, 360f)] public float AttackViewAngle { get; private set; } = 120f;
        [field: SerializeField, Min(0f)] public float HearingDistance { get; private set; } = 4f;
        [field: SerializeField] public bool UseLineOfSight { get; private set; } = true;
        [field: SerializeField] public LayerMask VisionOccluderMask { get; private set; } = Physics.DefaultRaycastLayers;
        [field: FormerlySerializedAs("<DrawVisionForAllMen>k__BackingField")]
        [field: SerializeField] public bool DrawVisionForAll { get; private set; }
    }
}
