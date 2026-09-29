using VContainer;
using VContainer.Unity;
using UnityEngine;
using Zombies.Loading;

public class LoadSceneLifetimeScope : LifetimeScope
{
    [SerializeField] private LoadSceneUI loadSceneUi;

    protected override void Awake()
    {
        var projectScope = ProjectLifetimeScope.Instance;
        if (projectScope == null)
        {
            Debug.LogError("ProjectLifetimeScope was not initialized.", this);
            return;
        }

        parentReference.Object = projectScope;
        base.Awake();
    }

    protected override void Configure(IContainerBuilder builder)
    {
        if (loadSceneUi == null)
        {
            Debug.LogError("LoadSceneUI is not assigned to LoadSceneLifetimeScope.", this);
            return;
        }

        builder.RegisterComponent(loadSceneUi);
    }
}
