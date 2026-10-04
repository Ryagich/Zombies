using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zombies.UI
{
    public sealed class PlayPageHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text TitleText { get; private set; }
        [field: SerializeField] public TMP_Text TimeText { get; private set; }
        [field: SerializeField] public Button PauseButton { get; private set; }
        [field: SerializeField] public TMP_Text BrainCountText { get; private set; }
        [field: SerializeField] public Image BrainRestoreFill { get; private set; }
        [field: SerializeField] public RectTransform ZombiesListContent { get; private set; }
    }
}
