using VContainer;
using VContainer.Unity;
using UnityEngine;
using Zombies.UI;

public class MenuLifetimeScope : LifetimeScope
{
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
        builder.Register<MenuPage>(Lifetime.Singleton);
        builder.RegisterEntryPoint<MenuController>().AsSelf();
    }
}
