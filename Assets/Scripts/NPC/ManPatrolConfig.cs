using UnityEngine;

namespace Zombies.NPC
{
    [CreateAssetMenu(fileName = "ManPatrolConfig", menuName = "Configs/NPC/Man Patrol Config")]
    public sealed class ManPatrolConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)]
        public float WaitAtPointSeconds { get; private set; } = 2f;
    }
}
