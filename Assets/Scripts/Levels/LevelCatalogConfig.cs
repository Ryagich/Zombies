using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Zombies.Levels
{
    [CreateAssetMenu(fileName = "LevelCatalogConfig", menuName = "Zombies/Level Catalog Config")]
    public sealed class LevelCatalogConfig : ScriptableObject
    {
        [field: SerializeField] public List<LevelDefinition> Levels { get; private set; } = new();
    }

    [Serializable]
    public sealed class LevelDefinition
    {
        [field: SerializeField] public GameObject LevelPrefab { get; private set; }
        [field: SerializeField] public bool IsUnlocked { get; private set; }
        [field: SerializeField] public LocalizedString LevelName { get; private set; }

        public LevelDefinition(GameObject levelPrefab, bool isUnlocked, LocalizedString levelName)
        {
            LevelPrefab = levelPrefab;
            IsUnlocked = isUnlocked;
            LevelName = levelName;
        }

        public void Unlock() => IsUnlocked = true;
    }
}
