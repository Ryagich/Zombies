using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Zombies.Levels
{
    [CreateAssetMenu(fileName = "LevelCatalogConfig", menuName = "Zombies/Level Catalog Config")]
    public sealed class LevelCatalogConfig : ScriptableObject
    {
        [field: SerializeField] public List<LocationDefinition> Locations { get; private set; } = new();
    }

    [Serializable]
    public sealed class LocationDefinition
    {
        [field: SerializeField] public LocalizedString LocationName { get; private set; }
        [field: SerializeField] public List<LevelDefinition> Levels { get; private set; } = new();
    }

    [Serializable]
    public sealed class LevelDefinition
    {
        public const int DefaultCompletionTimeSeconds = 5 * 60;

        [field: SerializeField] public GameObject LevelPrefab { get; private set; }
        [field: SerializeField] public bool IsUnlocked { get; private set; }
        [field: SerializeField, Min(1)] public int CompletionTimeSeconds { get; private set; } = DefaultCompletionTimeSeconds;

        public LevelDefinition(
            GameObject levelPrefab,
            bool isUnlocked,
            int completionTimeSeconds = DefaultCompletionTimeSeconds)
        {
            LevelPrefab = levelPrefab;
            IsUnlocked = isUnlocked;
            CompletionTimeSeconds = completionTimeSeconds;
        }

        public void Unlock() => IsUnlocked = true;
    }
}
