using DG.Tweening;
using UnityEngine;

namespace Zombies.Levels
{
    [CreateAssetMenu(fileName = "MapInteractionConfig", menuName = "Zombies/Map Interaction Config")]
    public sealed class MapInteractionConfig : ScriptableObject
    {
        [field: SerializeField, Min(1f)] public float HoverScale { get; private set; } = 1.1f;
        [field: SerializeField, Min(0f)] public float HoverDuration { get; private set; } = 0.15f;
        [field: SerializeField] public Ease HoverEase { get; private set; } = Ease.OutQuad;
    }
}
