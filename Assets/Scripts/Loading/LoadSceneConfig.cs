using UnityEngine;
using UnityEngine.Localization;

namespace Zombies.Loading
{
    [CreateAssetMenu(fileName = "LoadSceneConfig", menuName = "Zombies/Loading Config")]
    public sealed class LoadSceneConfig : ScriptableObject
    {
        [field: SerializeField] public string LoadSceneName { get; private set; } = "Load Scene";
        [field: SerializeField] public string MenuSceneName { get; private set; } = "Menu";
        [field: SerializeField] public LocalizedString PressAnyKeyText { get; private set; } = new("Tables", "LoadScene_PressAnyKey");

        [field: Space]
        [field: SerializeField, Min(0.01f)] public float AnimationFrameSeconds { get; private set; } = 0.18f;
        [field: SerializeField, Min(0.01f)] public float ReadyTextBlinkSpeed { get; private set; } = 1.25f;
        [field: SerializeField, Range(0f, 1f)] public float ReadyTextMinAlpha { get; private set; } = 0.45f;
        [field: SerializeField, Range(0f, 1f)] public float ReadyTextMaxAlpha { get; private set; } = 0.95f;
    }
}
