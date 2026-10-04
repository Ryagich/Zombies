using VContainer;
using VContainer.Unity;
using MessagePipe;
using Zombies.GameModes;
using Zombies.Input;
using Zombies.Navigation;
using Zombies.Levels;
using Unity.AI.Navigation;
using UnityEngine;
using Zombies.NPC;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private CanvasLifetimeScope canvasScopePrefab;
    [SerializeField] private NavMeshSurface navMeshSurfacePrefab;
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject mapPrefab;
    private Camera gameCamera;

    public void SetGameCamera(Camera camera) => gameCamera = camera;
    public NavMeshSurface NavMeshSurfacePrefab => navMeshSurfacePrefab;
    public GameObject ZombiePrefab => zombiePrefab;
    public GameObject MapPrefab => mapPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        var options = builder.RegisterMessagePipe();
        builder.RegisterMessageBroker<ChangeGameModeRequest>(options);
        builder.RegisterMessageBroker<GameModeChangedMessage>(options);
        builder.RegisterMessageBroker<PauseInputMessage>(options);
        builder.RegisterMessageBroker<CameraEdgeMoveMessage>(options);
        builder.RegisterMessageBroker<CameraDragMoveMessage>(options);
        builder.RegisterMessageBroker<CameraKeyboardMoveMessage>(options);
        builder.RegisterMessageBroker<GroundClickedMessage>(options);
        builder.RegisterMessageBroker<LocationSelectedMessage>(options);
        builder.RegisterMessageBroker<LocationHoverMessage>(options);
        if (gameCamera == null)
        {
            Debug.LogError("Game camera is not assigned to GameLifetimeScope.", this);
        }
        else
        {
            builder.RegisterInstance(gameCamera);
        }
        builder.RegisterEntryPoint<GameModeController>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<InputHandler>(Lifetime.Singleton);
        builder.RegisterEntryPoint<CursorController>(Lifetime.Singleton);
        builder.RegisterEntryPoint<CameraMovementController>(Lifetime.Singleton).AsSelf();
        builder.Register<GameNavMeshController>(Lifetime.Singleton);
        builder.Register<HumansController>(Lifetime.Singleton).AsSelf();
        builder.Register<ZombiesController>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<BrainController>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<ZombieSelectionController>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<LevelCompletionController>(Lifetime.Singleton);
        builder.RegisterEntryPoint<MapController>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<LevelController>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<LevelTimerController>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<GroundClickZombieSpawner>(Lifetime.Singleton);
    }

    public void BuildCanvasScope()
    {
        if (canvasScopePrefab == null)
        {
            Debug.LogError("CanvasLifetimeScope prefab is not assigned to GameLifetimeScope.", this);
            return;
        }

        var canvasScope = Instantiate(canvasScopePrefab);
        canvasScope.gameObject.SetActive(false);
        canvasScope.parentReference.Object = this;
        canvasScope.gameObject.SetActive(true);
        canvasScope.Build();
    }
}
