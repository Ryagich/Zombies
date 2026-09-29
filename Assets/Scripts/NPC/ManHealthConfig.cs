using UnityEngine;

namespace Zombies.NPC
{
    [CreateAssetMenu(fileName = "HealthConfig", menuName = "Configs/NPC/Health Config")]
    public sealed class HealthConfig : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float MaxHp { get; private set; } = 100f;
    }
}
