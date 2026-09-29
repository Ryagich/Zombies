using UnityEngine;

namespace Zombies.NPC
{
    [CreateAssetMenu(fileName = "AttackConfig", menuName = "Configs/NPC/Attack Config")]
    public sealed class AttackConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float Damage { get; private set; } = 10f;
        [field: SerializeField, Min(0.01f)] public float IntervalSeconds { get; private set; } = 1f;
    }
}
