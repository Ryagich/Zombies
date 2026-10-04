using UnityEngine;

namespace Zombies.NPC
{
    [CreateAssetMenu(fileName = "ZombieConfig", menuName = "Zombies/NPC/Zombie Config")]
    public sealed class ZombieConfig : ScriptableObject
    {
        [field: SerializeField, Min(0)] public int BrainCost { get; private set; }
        [field: SerializeField, Min(0f)] public float SpawnCooldown { get; private set; }
        [field: SerializeField] public GameObject ZombiePrefab { get; private set; }
        // An absent icon is intentional and supported by the UI.
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
