using UnityEngine;
using UnityEngine.AI;
using VContainer;
using VContainer.Unity;
using Zombies.StateMachine.Graph;
using Zombies.NPC;

public class ZombieLifetimeScope : LifetimeScope
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private StateMachineGraph graph;
    [SerializeField] private Collider damageZone;
    [SerializeField] private HealthConfig healthConfig;
    [SerializeField] private AttackConfig attackConfig;

    public NpcHealth Hp { get; private set; }
    public Collider BodyCollider => GetComponent<Collider>();
    public AttackConfig AttackConfig => attackConfig;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(agent);
        builder.RegisterInstance(graph);
        builder.RegisterComponent(damageZone);
        Hp = new NpcHealth(healthConfig);
        builder.RegisterInstance(Hp);
        builder.RegisterInstance(attackConfig);
        builder.RegisterEntryPoint<ZombieCombatEntryPoint>(Lifetime.Singleton);
    }
}
