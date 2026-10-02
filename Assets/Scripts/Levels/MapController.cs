using MessagePipe;
using UnityEngine;
using VContainer.Unity;
using Zombies.Bootstrap;
using Zombies.CameraSystem;

namespace Zombies.Levels
{
    /// <summary>Creates the map for the game scene. Levels are instantiated later, after selection.</summary>
    public sealed class MapController : IStartable, System.IDisposable
    {
        private readonly BootCompletion bootCompletion;
        private readonly GameLifetimeScope gameScope;
        private readonly LevelCatalogConfig levelCatalogConfig;
        private readonly MapInteractionConfig mapInteractionConfig;
        private readonly LevelSelectionState selectionState;
        private readonly IPublisher<LocationSelectedMessage> locationSelectedPublisher;
        private readonly IPublisher<LocationHoverMessage> locationHoverPublisher;
        private readonly ISubscriber<LocationSelectedMessage> locationSelectedSubscriber;
        private readonly Zombies.Input.CameraMovementController cameraMovementController;
        private GameObject mapObject;
        private System.IDisposable locationSelectedSubscription;

        public MapController(
            BootCompletion bootCompletion,
            GameLifetimeScope gameScope,
            LevelCatalogConfig levelCatalogConfig,
            MapInteractionConfig mapInteractionConfig,
            LevelSelectionState selectionState,
            IPublisher<LocationSelectedMessage> locationSelectedPublisher,
            IPublisher<LocationHoverMessage> locationHoverPublisher,
            ISubscriber<LocationSelectedMessage> locationSelectedSubscriber,
            Zombies.Input.CameraMovementController cameraMovementController)
        {
            this.bootCompletion = bootCompletion;
            this.gameScope = gameScope;
            this.levelCatalogConfig = levelCatalogConfig;
            this.mapInteractionConfig = mapInteractionConfig;
            this.selectionState = selectionState;
            this.locationSelectedPublisher = locationSelectedPublisher;
            this.locationHoverPublisher = locationHoverPublisher;
            this.locationSelectedSubscriber = locationSelectedSubscriber;
            this.cameraMovementController = cameraMovementController;
        }

        public async void Start()
        {
            locationSelectedSubscription = locationSelectedSubscriber.Subscribe(_ => RemoveMap());
            await bootCompletion.WaitAsync();

            ShowMap();
        }

        public void ShowMap()
        {
            if (mapObject != null)
                return;

            if (gameScope.MapPrefab == null)
            {
                Debug.LogError("Map prefab is not assigned to GameLifetimeScope.");
                return;
            }

            mapObject = Object.Instantiate(gameScope.MapPrefab, gameScope.transform);
            mapObject.name = gameScope.MapPrefab.name;
            var holder = mapObject.GetComponent<MapHolder>();
            if (holder == null)
            {
                Debug.LogError("Map prefab must contain MapHolder.", mapObject);
                return;
            }

            holder.Initialize(
                levelCatalogConfig,
                mapInteractionConfig,
                selectionState,
                locationSelectedPublisher,
                locationHoverPublisher);
            cameraMovementController.SetBounds(mapObject.GetComponentInChildren<CameraBounds>());
        }

        public void Dispose()
        {
            locationSelectedSubscription?.Dispose();
            RemoveMap();
        }

        private void RemoveMap()
        {
            if (mapObject == null)
                return;

            mapObject.SetActive(false);
            Object.Destroy(mapObject);
            mapObject = null;
        }
    }
}
