using UnityEngine;
using VContainer.Unity;
using Zombies.Bootstrap;
using Zombies.Navigation;
using Zombies.CameraSystem;
using Zombies.Input;
using Zombies.NPC;

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
        private readonly GameLifetimeScope gameScope;

        private GameObject currentLevelObject;
        public LevelController(
            BootCompletion bootCompletion,
            GameNavMeshController navMeshController,
            LevelSelectionState selectionState,
            CameraMovementController cameraMovementController,
            GameLifetimeScope gameScope,
            HumansController humansController)
        {
            this.bootCompletion = bootCompletion;
            this.navMeshController = navMeshController;
            this.selectionState = selectionState;
            this.cameraMovementController = cameraMovementController;
            this.humansController = humansController;
            this.gameScope = gameScope;
            parent = gameScope.transform;
        }

        public async void Start()
        {
            await bootCompletion.WaitAsync();

            selectionState.LevelChanged += CreateLevel;
            selectionState.SelectLastUnlocked();
        }

        public void Dispose()
        {
            selectionState.LevelChanged -= CreateLevel;

            if (currentLevelObject != null)
            {
                humansController.Clear();
                // Object.Destroy is deferred until the end of the frame. Hide
                // the previous level now so it is excluded from the new bake.
                currentLevelObject.SetActive(false);
                Object.Destroy(currentLevelObject);
            }
        }

        private void CreateLevel(LevelDefinition level)
        {
            if (level == null || level.LevelPrefab == null)
            {
                Debug.LogError("Selected level has no level prefab.");
                return;
            }

            if (currentLevelObject != null)
            {
                humansController.Clear();
                // Destroy is deferred; disable first to exclude it from this bake.
                currentLevelObject.SetActive(false);
                Object.Destroy(currentLevelObject);
            }

            currentLevelObject = Object.Instantiate(level.LevelPrefab, parent);
            currentLevelObject.name = level.LevelPrefab.name;
            var people = currentLevelObject.GetComponent<LevelHumansRegistry>()?.People;
            BuildHumanScopes(people);
            humansController.SetLevelHumans(people);
            ConfigureCameraBounds(currentLevelObject);
            navMeshController.Rebuild();
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
