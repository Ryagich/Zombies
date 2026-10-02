using System;
using System.Collections.Generic;
using MessagePipe;
using UnityEngine;

namespace Zombies.Levels
{
    /// <summary>Maps scene objects to locations from the level catalog.</summary>
    public sealed class MapHolder : MonoBehaviour
    {
        [SerializeField] private List<MapLocationEntry> interactiveObjects = new();

        private LevelCatalogConfig locationCatalog;
        private MapInteractionConfig interactionConfig;
        private LevelSelectionState selectionState;
        private IPublisher<LocationSelectedMessage> locationSelectedPublisher;
        private IPublisher<LocationHoverMessage> locationHoverPublisher;

        public IList<MapLocationEntry> InteractiveObjects => interactiveObjects;

        public void Initialize(
            LevelCatalogConfig catalog,
            MapInteractionConfig mapInteractionConfig,
            LevelSelectionState state,
            IPublisher<LocationSelectedMessage> publisher,
            IPublisher<LocationHoverMessage> hoverPublisher)
        {
            locationCatalog = catalog;
            interactionConfig = mapInteractionConfig;
            selectionState = state;
            locationSelectedPublisher = publisher;
            locationHoverPublisher = hoverPublisher;

            for (var index = 0; index < interactiveObjects.Count; index++)
            {
                var target = interactiveObjects[index]?.Target;
                if (target == null)
                    continue;

                var interaction = target.GetComponent<MapLocationInteraction>();
                if (interaction == null)
                    interaction = target.AddComponent<MapLocationInteraction>();

                interaction.Initialize(this, index);
            }
        }

        public MapInteractionConfig InteractionConfig => interactionConfig;

        public bool IsLocationAvailable(int entryIndex)
        {
            return TryGetLocationIndex(entryIndex, out var locationIndex)
                   && selectionState != null
                   && selectionState.HasUnlockedLevel(locationIndex);
        }

        public void SelectLocation(int entryIndex)
        {
            if (!TryGetLocationIndex(entryIndex, out var locationIndex)
                || selectionState == null
                || !selectionState.TrySelectLastUnlockedInLocation(locationIndex))
                return;

            locationSelectedPublisher?.Publish(new LocationSelectedMessage(locationIndex));
        }

        public void SetHoveredLocation(int entryIndex)
        {
            locationHoverPublisher?.Publish(new LocationHoverMessage(
                TryGetLocationIndex(entryIndex, out var locationIndex) ? locationIndex : -1));
        }

        private bool TryGetLocationIndex(int entryIndex, out int locationIndex)
        {
            locationIndex = -1;
            if (entryIndex < 0 || entryIndex >= interactiveObjects.Count || locationCatalog?.Locations == null)
                return false;

            locationIndex = interactiveObjects[entryIndex].LocationIndex;
            return locationIndex >= 0 && locationIndex < locationCatalog.Locations.Count;
        }
    }

    [Serializable]
    public sealed class MapLocationEntry
    {
        [field: SerializeField] public GameObject Target { get; private set; }
        [field: SerializeField, HideInInspector] public int LocationIndex { get; private set; } = -1;
    }
}
