using System;
using UnityEngine;

namespace Zombies.Levels
{
    /// <summary>
    /// Shared level selection state. Level navigation is constrained to the
    /// currently selected location.
    /// </summary>
    public sealed class LevelSelectionState
    {
        private readonly LevelCatalogConfig catalog;
        private int currentFlatIndex = -1;

        public event Action<LevelDefinition> LevelChanged;

        public LevelDefinition CurrentLevel => TryGetEntry(currentFlatIndex, out _, out _, out var level)
            ? level
            : null;

        /// <summary>Zero-based index of the level inside its location.</summary>
        public int CurrentLevelIndex => TryGetEntry(currentFlatIndex, out _, out var levelIndex, out _)
            ? levelIndex
            : -1;

        public int CurrentLocationIndex => TryGetEntry(currentFlatIndex, out var locationIndex, out _, out _)
            ? locationIndex
            : -1;

        public bool CanSelectPrevious => CurrentLocationLevelCount > 1;
        public bool CanSelectNext => CurrentLocationLevelCount > 1;

        private int CurrentLocationLevelCount => CurrentLocationIndex >= 0
            ? catalog?.Locations?[CurrentLocationIndex]?.Levels?.Count ?? 0
            : 0;

        private int LevelCount
        {
            get
            {
                if (catalog?.Locations == null)
                    return 0;

                var count = 0;
                foreach (var location in catalog.Locations)
                    count += location?.Levels?.Count ?? 0;

                return count;
            }
        }

        public LevelSelectionState(LevelCatalogConfig catalog)
        {
            this.catalog = catalog;
        }

        public void SelectLastUnlocked()
        {
            if (LevelCount == 0)
            {
                Debug.LogError("LevelCatalogConfig has no levels in its locations.");
                return;
            }

            for (var index = LevelCount - 1; index >= 0; index--)
            {
                if (TryGetEntry(index, out _, out _, out var level) && level != null && level.IsUnlocked)
                {
                    Select(index);
                    return;
                }
            }

            Debug.LogError("LevelCatalogConfig has no unlocked levels. Unlock at least one level in the catalog.");
        }

        /// <summary>Returns whether a location contains at least one unlocked level.</summary>
        public bool HasUnlockedLevel(int locationIndex)
        {
            if (locationIndex < 0 || catalog?.Locations == null || locationIndex >= catalog.Locations.Count)
                return false;

            var levels = catalog.Locations[locationIndex]?.Levels;
            if (levels == null)
                return false;

            foreach (var level in levels)
            {
                if (level != null && level.IsUnlocked)
                    return true;
            }

            return false;
        }

        /// <summary>Selects the last unlocked level belonging to a location.</summary>
        public bool TrySelectLastUnlockedInLocation(int locationIndex)
        {
            if (!HasUnlockedLevel(locationIndex))
                return false;

            var flatIndex = 0;
            for (var currentLocationIndex = 0; currentLocationIndex < catalog.Locations.Count; currentLocationIndex++)
            {
                var levels = catalog.Locations[currentLocationIndex]?.Levels;
                if (levels == null)
                    continue;

                if (currentLocationIndex == locationIndex)
                {
                    for (var levelIndex = levels.Count - 1; levelIndex >= 0; levelIndex--)
                    {
                        var level = levels[levelIndex];
                        if (level != null && level.IsUnlocked)
                        {
                            Select(flatIndex + levelIndex);
                            return CurrentLocationIndex == locationIndex;
                        }
                    }
                }

                flatIndex += levels.Count;
            }

            return false;
        }

        public void SelectPrevious()
        {
            if (!CanSelectPrevious)
                return;

            SelectLevelInCurrentLocation(CurrentLevelIndex == 0
                ? CurrentLocationLevelCount - 1
                : CurrentLevelIndex - 1);
        }

        public void SelectNext()
        {
            if (!CanSelectNext)
                return;

            SelectLevelInCurrentLocation((CurrentLevelIndex + 1) % CurrentLocationLevelCount);
        }

        private void SelectLevelInCurrentLocation(int levelIndex)
        {
            if (!TryGetFlatIndex(CurrentLocationIndex, levelIndex, out var flatIndex))
                return;

            Select(flatIndex);
        }

        public void CompleteCurrentLevel()
        {
            if (currentFlatIndex < 0)
                return;

            if (TryGetEntry(currentFlatIndex + 1, out _, out _, out var nextLevel) && nextLevel != null)
                nextLevel.Unlock();

            LevelChanged?.Invoke(CurrentLevel);
        }

        private void Select(int flatIndex)
        {
            if (!TryGetEntry(flatIndex, out _, out _, out var level) || level == null || level.LevelPrefab == null)
            {
                Debug.LogError($"LevelCatalogConfig entry at index {flatIndex} has no level prefab.");
                return;
            }

            currentFlatIndex = flatIndex;
            LevelChanged?.Invoke(level);
        }

        private bool TryGetEntry(int flatIndex, out int locationIndex, out int levelIndex, out LevelDefinition level)
        {
            locationIndex = -1;
            levelIndex = -1;
            level = null;

            if (flatIndex < 0 || catalog?.Locations == null)
                return false;

            var index = 0;
            for (var currentLocationIndex = 0; currentLocationIndex < catalog.Locations.Count; currentLocationIndex++)
            {
                var location = catalog.Locations[currentLocationIndex];
                if (location?.Levels == null)
                    continue;

                for (var currentLevelIndex = 0; currentLevelIndex < location.Levels.Count; currentLevelIndex++)
                {
                    if (index++ != flatIndex)
                        continue;

                    locationIndex = currentLocationIndex;
                    levelIndex = currentLevelIndex;
                    level = location.Levels[currentLevelIndex];
                    return true;
                }
            }

            return false;
        }

        private bool TryGetFlatIndex(int targetLocationIndex, int targetLevelIndex, out int flatIndex)
        {
            flatIndex = -1;
            if (targetLocationIndex < 0 || targetLevelIndex < 0 || catalog?.Locations == null)
                return false;

            for (var locationIndex = 0; locationIndex < catalog.Locations.Count; locationIndex++)
            {
                var levels = catalog.Locations[locationIndex]?.Levels;
                if (levels == null)
                    continue;

                if (locationIndex == targetLocationIndex)
                {
                    if (targetLevelIndex >= levels.Count)
                        return false;

                    flatIndex += targetLevelIndex + 1;
                    return true;
                }

                flatIndex += levels.Count;
            }

            return false;
        }
    }
}
