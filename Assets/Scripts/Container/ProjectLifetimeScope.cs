using VContainer;
using VContainer.Unity;
using UnityEngine;
using Zombies.Bootstrap;
using Zombies.UI;
using Zombies.Loading;
using Zombies.Input;
using Zombies.Levels;
using Zombies.GameModes;
using Zombies.NPC;

public class ProjectLifetimeScope : LifetimeScope
{
    public static ProjectLifetimeScope Instance { get; private set; }

    [field: SerializeField] public UIConfig UIConfig { get; private set; }
    [field: SerializeField] public LoadSceneConfig LoadSceneConfig { get; private set; }
    [field: SerializeField] public InputConfig InputConfig { get; private set; }
    [field: SerializeField] public CameraMovementConfig CameraMovementConfig { get; private set; }
    [field: SerializeField] public LevelCatalogConfig LevelCatalogConfig { get; private set; }
    [field: SerializeField] public MapInteractionConfig MapInteractionConfig { get; private set; }
    [field: SerializeField] public BrainConfig BrainConfig { get; private set; }
    [field: SerializeField] public ZombieStorage ZombieStorage { get; private set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }

    protected override void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        base.OnDestroy();
    }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(UIConfig).AsSelf();
            builder.RegisterInstance(LoadSceneConfig).AsSelf();
            builder.RegisterInstance(InputConfig).AsSelf();
            builder.RegisterInstance(CameraMovementConfig).AsSelf();
            builder.RegisterInstance(LevelCatalogConfig).AsSelf();
            builder.RegisterInstance(MapInteractionConfig).AsSelf();
            builder.RegisterInstance(BrainConfig).AsSelf();
            builder.RegisterInstance(ZombieStorage).AsSelf();
            builder.Register<LevelSelectionState>(Lifetime.Singleton);
            if (UIConfig == null || UIConfig.CanvasPrefab == null)
            {
                Debug.LogError("UIConfig and its CanvasPrefab must be assigned to ProjectLifetimeScope.", this);
            }
            else
            {
                builder.RegisterComponentInNewPrefab(UIConfig.CanvasPrefab, Lifetime.Singleton).As<Canvas>();
            }
            builder.Register<BootCompletion>(Lifetime.Singleton).AsSelf();
            builder.Register<GameModeRequestService>(Lifetime.Singleton);
            builder.Register<SceneLoadingService>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<ProjectBootloader>(Lifetime.Singleton);
    }
}
