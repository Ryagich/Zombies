using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;
using Zombies.StateMachine;
using Zombies.StateMachine.Graph;

namespace Zombies.NPC
{
    public sealed class ManStateMachineEntryPoint : IStartable, ITickable, System.IDisposable
    {
        private readonly Zombies.StateMachine.StateMachine stateMachine;
        private readonly ManLifetimeScope scope;
        private readonly HumansController humans;
        private readonly NavMeshAgent agent;
        private readonly Collider damageZone;
        private readonly Vision vision;
        private readonly AttackConfig attackConfig;
        private readonly ZombiesController zombies;
        private ZombieLifetimeScope target;
        private Vector3 returnPosition;
        private bool returning;
        private float nextAttackTime;

        public ManStateMachineEntryPoint(
            ManLifetimeScope scope,
            StateMachineGraph graph,
            NavMeshAgent agent,
            Collider damageZone,
            Vision vision,
            AttackConfig attackConfig,
            NpcHealth hpController,
            HumansController humans,
            ZombiesController zombies)
        {
            this.scope = scope;
            this.humans = humans;
            this.agent = agent;
            this.damageZone = damageZone;
            this.vision = vision;
            this.attackConfig = attackConfig;
            this.zombies = zombies;
            var context = new StateMachineContext { Owner = scope.gameObject };
            context.SetService(scope);
            context.SetService(agent);
            context.SetService(hpController);
            context.SetService(scope.PatrolConfig);
            stateMachine = new Zombies.StateMachine.StateMachine(graph, context);
        }

        public void Start()
        {
            scope.Hp.Died += OnDied;
            stateMachine.Start();
        }
        public void Tick()
        {
            if (scope.Hp.IsDead)
                return;

            var visibleZombie = FindDetectedZombie();
            if (visibleZombie != null)
            {
                if (target == null)
                    returnPosition = scope.transform.position;

                target = visibleZombie;
                returning = false;
                FightTarget();
                return;
            }

            if (target != null)
            {
                target = null;
                returning = true;
            }

            if (returning)
            {
                if (MoveTo(returnPosition))
                    returning = false;
                return;
            }

            stateMachine.Tick(Time.deltaTime);
        }

        public void Dispose() => scope.Hp.Died -= OnDied;

        private void OnDied()
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            humans.MarkDead(scope);
            Object.Destroy(scope.gameObject);
        }

        private ZombieLifetimeScope FindDetectedZombie()
        {
            ZombieLifetimeScope closest = null;
            var closestDistance = float.PositiveInfinity;
            foreach (var zombie in zombies.Zombies)
            {
                if (zombie == null || zombie.Hp == null || zombie.Hp.IsDead || !zombie.gameObject.activeInHierarchy)
                    continue;
                if (!vision.IsInView(zombie.transform) && !vision.IsInHearingRange(zombie.transform))
                    continue;

                var distance = (zombie.transform.position - scope.transform.position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closest = zombie;
                    closestDistance = distance;
                }
            }
            return closest;
        }

        private void FightTarget()
        {
            if (target == null)
                return;
            if (IsInDamageZone(target.BodyCollider))
            {
                Stop();
                if (attackConfig != null && Time.time >= nextAttackTime)
                {
                    target.Hp.TakeDamage(attackConfig.Damage);
                    nextAttackTime = Time.time + attackConfig.IntervalSeconds;
                }
                return;
            }
            MoveTo(target.transform.position);
        }

        private bool MoveTo(Vector3 destination)
        {
            if (agent == null || !agent.isOnNavMesh)
                return true;
            agent.isStopped = false;
            agent.SetDestination(destination);
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        }

        private bool IsInDamageZone(Collider targetCollider)
        {
            if (damageZone == null || targetCollider == null || !damageZone.enabled || !targetCollider.enabled)
                return false;

            return Physics.ComputePenetration(
                damageZone, damageZone.transform.position, damageZone.transform.rotation,
                targetCollider, targetCollider.transform.position, targetCollider.transform.rotation,
                out _, out _);
        }

        private void Stop()
        {
            if (agent == null || !agent.isOnNavMesh)
                return;
            agent.isStopped = true;
            if (agent.hasPath)
                agent.ResetPath();
        }
    }
}
