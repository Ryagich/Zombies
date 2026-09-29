using UnityEngine;

namespace Zombies.UI
{
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Zombies/UI Config")]
    public sealed class UIConfig : ScriptableObject
    {
        [field: SerializeField] public Canvas CanvasPrefab { get; private set; }
        [field: SerializeField] public RectTransform ContentPrefab { get; private set; }
        [field: SerializeField] public RectTransform MenuPagePrefab { get; private set; }
        [field: SerializeField] public RectTransform LevelPreparationPagePrefab { get; private set; }
        [field: SerializeField] public RectTransform PlayPagePrefab { get; private set; }
        [field: SerializeField] public RectTransform PausePagePrefab { get; private set; }
        [field: SerializeField] public RectTransform VictoryPagePrefab { get; private set; }
    }
}
