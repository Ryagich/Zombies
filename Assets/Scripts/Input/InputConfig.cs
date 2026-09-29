using UnityEngine;
using UnityEngine.InputSystem;

namespace Zombies.Input
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "Zombies/Input Config")]
    public sealed class InputConfig : ScriptableObject
    {
        [field: SerializeField] public InputActionReference PointerPosition { get; private set; }
        [field: SerializeField] public InputActionReference PointerDelta { get; private set; }
        [field: SerializeField] public InputActionReference CameraDrag { get; private set; }
        [field: SerializeField] public InputActionReference Pause { get; private set; }
        [field: SerializeField] public LayerMask GroundClickLayers { get; private set; }
        [field: SerializeField] public InputActionReference CameraKeyboardMove { get; private set; }
    }
}
