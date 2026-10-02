using TMPro;
using UnityEngine;

namespace Zombies.UI
{
    public sealed class MapPageHolder : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text TitleText { get; private set; }
    }
}
