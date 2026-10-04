using UnityEngine;

namespace Zombies.NPC
{
    [CreateAssetMenu(fileName = "BrainConfig", menuName = "Zombies/NPC/Brain Config")]
    public sealed class BrainConfig : ScriptableObject
    {
        [field: SerializeField, Min(1)] public int MaxBrains { get; private set; } = 10;
        [field: SerializeField, Min(0.01f)] public float SecondsPerBrain { get; private set; } = 1f;
    }
}
