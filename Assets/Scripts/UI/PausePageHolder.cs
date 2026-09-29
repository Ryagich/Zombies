using UnityEngine;
using UnityEngine.UI;

namespace Zombies.UI
{
    public sealed class PausePageHolder : MonoBehaviour
    {
        [field: SerializeField] public Button ResumeButton { get; private set; }
    }
}
