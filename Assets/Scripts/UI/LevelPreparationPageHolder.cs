using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Zombies.UI
{
    public sealed class LevelPreparationPageHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text TitleText { get; private set; }
        [field: SerializeField] public TMP_Text PeopleText { get; private set; }
        [field: SerializeField] public TMP_Text TimeText { get; private set; }
        [field: SerializeField] public Button LeftButton { get; private set; }
        [field: SerializeField] public Button RightButton { get; private set; }
        [field: SerializeField] public Button PlayButton { get; private set; }
        [field: SerializeField] public Button ToMapButton { get; private set; }
        [field: SerializeField] public Image CloseImage { get; private set; }
    }
}
