using System;
using UnityEngine;

namespace Zombies.Levels
{
    /// <summary>
    /// Shared level selection state. It contains no scene objects, so both the
    /// game scope and the canvas scope can use it through ProjectLifetimeScope.
    /// </summary>
    public sealed class LevelSelectionState
    {
        private readonly LevelCatalogConfig catalog;
        private int currentIndex = -1;

        public event Action<LevelDefinition> LevelChanged;

        public LevelDefinition CurrentLevel => currentIndex >= 0 && currentIndex < catalog.Levels.Count
            ? catalog.Levels[currentIndex]
            : null;
        // Level browsing is cyclic: both arrows remain usable whenever the
        // catalog contains at least two levels.
        public bool CanSelectPrevious => catalog != null && catalog.Levels != null && catalog.Levels.Count > 1;
        public bool CanSelectNext => catalog != null && catalog.Levels != null && catalog.Levels.Count > 1;

        public LevelSelectionState(LevelCatalogConfig catalog)
        {
            this.catalog = catalog;
        }

        public void SelectLastUnlocked()
        {
            if (catalog == null || catalog.Levels == null)
            {
                Debug.LogError("LevelCatalogConfig is not assigned to ProjectLifetimeScope.");
                return;
            }

            var lastUnlockedIndex = catalog.Levels.FindLastIndex(level => level != null && level.IsUnlocked);
            if (lastUnlockedIndex < 0)
            {
                Debug.LogError("LevelCatalogConfig has no unlocked levels. Unlock at least one level in the catalog.");
                return;
            }

            Select(lastUnlockedIndex);
        }

        public void SelectPrevious()
        {
            if (!CanSelectPrevious || currentIndex < 0)
                return;

            Select(currentIndex == 0 ? catalog.Levels.Count - 1 : currentIndex - 1);
        }

        public void SelectNext()
        {
            if (!CanSelectNext || currentIndex < 0)
                return;

            Select(currentIndex == catalog.Levels.Count - 1 ? 0 : currentIndex + 1);
        }

        public void CompleteCurrentLevel()
        {
            if (currentIndex < 0 || catalog == null || catalog.Levels == null)
                return;

            var nextIndex = currentIndex + 1;
            if (nextIndex < catalog.Levels.Count && catalog.Levels[nextIndex] != null)
                catalog.Levels[nextIndex].Unlock();

            LevelChanged?.Invoke(CurrentLevel);
        }

        private void Select(int index)
        {
            var level = catalog.Levels[index];
            if (level == null || level.LevelPrefab == null)
            {
                Debug.LogError($"LevelCatalogConfig entry at index {index} has no level prefab.");
                return;
            }

            currentIndex = index;
            LevelChanged?.Invoke(level);
        }
    }
}
