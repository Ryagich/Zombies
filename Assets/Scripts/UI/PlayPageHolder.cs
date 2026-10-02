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
    }
}
