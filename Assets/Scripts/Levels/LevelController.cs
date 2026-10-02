using MessagePipe;
using UnityEngine;
using VContainer.Unity;
using Zombies.Bootstrap;
using Zombies.Navigation;
using Zombies.CameraSystem;
using Zombies.Input;
using Zombies.NPC;
using Zombies.GameModes;

namespace Zombies.Levels
{
    public sealed class LevelController : IStartable, System.IDisposable
    {
        private readonly BootCompletion bootCompletion;
        private readonly GameNavMeshController navMeshController;
        private readonly LevelSelectionState selectionState;
        private readonly CameraMovementController cameraMovementController;
        private readonly Transform parent;
        private readonly HumansController humansController;
        private readonly ZombiesController zombiesController;
        private readonly ISubscriber<GameModeChangedMessage> gameModeChanged;
        private readonly GameLifetimeScope gameScope;

        private GameObject currentLevelObject;
        private System.IDisposable gameModeSubscription;
        private bool resetPending;
        public LevelController(
            BootCompletion bootCompletion,
            GameNavMeshController navMeshController,
            LevelSelectionState selectionState,
            CameraMovementController cameraMovementController,
            GameLifetimeScope gameScope,
            HumansController humansController,
            ZombiesController zombiesController,
            ISubscriber<GameModeChangedMessage> gameModeChanged)
        {
            this.bootCompletion = bootCompletion;
            this.navMeshController = navMeshController;
            this.selectionState = selectionState;
            this.cameraMovementController = cameraMovementController;
            this.humansController = humansController;
            this.zombiesController = zombiesController;
            this.gameModeChanged = gameModeChanged;
            this.gameScope = gameScope;
            parent = gameScope.transform;
        }

        public async void Start()
        {
            await bootCompletion.WaitAsync();

            selectionState.LevelChanged += CreateLevel;
            gameModeSubscription = gameModeChanged.Subscribe(OnGameModeChanged);
        }

        public void Dispose()
        {
            selectionState.LevelChanged -= CreateLevel;
            gameModeSubscription?.Dispose();
            ClearLevel();
            zombiesController.Clear();
        }

        public void ClearCurrentLevel()
        {
            ClearLevel();
            zombiesController.Clear();
        }

        private void CreateLevel(LevelDefinition level)
        {
            if (resetPending)
                return;

            if (level == null || level.LevelPrefab == null)
            {
                Debug.LogError("Selected level has no level prefab.");
                return;
            }

            ClearLevel();
            zombiesController.Clear();

            currentLevelObject = Object.Instantiate(level.LevelPrefab, parent);
            currentLevelObject.name = level.LevelPrefab.name;
            var people = currentLevelObject.GetComponent<LevelHumansRegistry>()?.People;
            BuildHumanScopes(people);
            humansController.SetLevelHumans(people);
            ConfigureCameraBounds(currentLevelObject);
            navMeshController.Rebuild();
        }

        private void OnGameModeChanged(GameModeChangedMessage message)
        {
            switch (message.Mode)
            {
                case GameMode.Victory:
                case GameMode.Defeat:
                    resetPending = true;
                    ClearLevel();
                    zombiesController.Clear();
                    break;
                case GameMode.LevelPreview when resetPending:
                    resetPending = false;
                    CreateLevel(selectionState.CurrentLevel);
                    break;
            }
        }

        private void ClearLevel()
        {
            humansController.Clear();
            if (currentLevelObject == null)
                return;

            // Object.Destroy is deferred; disable first so this level cannot
            // participate in a later navmesh bake.
            currentLevelObject.SetActive(false);
            Object.Destroy(currentLevelObject);
            currentLevelObject = null;
        }

        private void BuildHumanScopes(System.Collections.Generic.IReadOnlyList<ManLifetimeScope> people)
        {
            if (people == null)
                return;

            foreach (var person in people)
            {
                if (person == null)
                    continue;

                person.parentReference.Object = gameScope;
                person.Build();
            }
        }

        private void ConfigureCameraBounds(GameObject levelObject)
        {
            CameraBounds bounds = levelObject.GetComponentInChildren<CameraBounds>();
            if (bounds == null)
            {
                Debug.LogWarning($"Level '{levelObject.name}' has no CameraBounds. Camera movement is unrestricted.", levelObject);
            }

            cameraMovementController.SetBounds(bounds);
        }
    }
}
