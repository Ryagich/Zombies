using VContainer;
using VContainer.Unity;
using Zombies.UI;

public class CanvasLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<MapPage>(Lifetime.Singleton);
        builder.Register<LevelPreparationPage>(Lifetime.Singleton);
        builder.Register<PlayPage>(Lifetime.Singleton);
        builder.Register<PausePage>(Lifetime.Singleton);
        builder.Register<VictoryPage>(Lifetime.Singleton);
        builder.Register<DefeatPage>(Lifetime.Singleton);
        builder.RegisterEntryPoint<PagesController>();
    }
}
