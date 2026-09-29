using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using VContainer;
using VContainer.Unity;
using Zombies.StateMachine.Graph;
using Zombies.NPC;

public class ManLifetimeScope : LifetimeScope
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private StateMachineGraph graph;
    [SerializeField] private Collider damageZone;
    [SerializeField] private Vision vision;
    [SerializeField] private HealthConfig healthConfig;
    [SerializeField] private AttackConfig attackConfig;
    [Tooltip("Points are visited in list order. At least two assigned points enable patrol.")]
    [SerializeField] private List<GameObject> patrolPoints = new();
    [SerializeField] private ManPatrolConfig patrolConfig;

    public ManPatrolConfig PatrolConfig => patrolConfig;
    public AttackConfig AttackConfig => attackConfig;
    public NpcHealth Hp { get; private set; }
    public Collider BodyCollider => GetComponent<Collider>();
    public IReadOnlyList<GameObject> PatrolPoints => patrolPoints;

    public bool CanPatrol
    {
        get
        {
            var count = 0;
            foreach (var point in patrolPoints)
                if (point != null)
                    count++;
            return count >= 2;
        }
    }

    public Transform GetPatrolPoint(int index)
    {
        var validCount = 0;
        foreach (var point in patrolPoints)
            if (point != null)
                validCount++;

        if (validCount == 0)
            return null;

        var targetIndex = ((index % validCount) + validCount) % validCount;
        var currentIndex = 0;
        foreach (var point in patrolPoints)
        {
            if (point == null)
                continue;

            if (currentIndex++ == targetIndex)
                return point.transform;
        }

        return null;
    }

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(agent);
        builder.RegisterInstance(graph);
        builder.RegisterComponent(damageZone);
        builder.RegisterComponent(vision);
        Hp = new NpcHealth(healthConfig);
        builder.RegisterInstance(Hp);
        builder.RegisterInstance(attackConfig);
        builder.RegisterEntryPoint<ManStateMachineEntryPoint>(Lifetime.Singleton);
    }
}
