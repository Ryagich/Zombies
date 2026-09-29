using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace Zombies.NPC
{
    public sealed class ZombieCombatEntryPoint : IStartable, ITickable, System.IDisposable
    {
        private readonly ZombieLifetimeScope scope;
        private readonly NavMeshAgent agent;
        private readonly Collider damageZone;
        private readonly AttackConfig attackConfig;
        private readonly HumansController humans;
        private readonly ZombiesController zombies;
        private ManLifetimeScope target;
        private float nextAttackTime;

        public ZombieCombatEntryPoint(ZombieLifetimeScope scope, NavMeshAgent agent, Collider damageZone, AttackConfig attackConfig, HumansController humans, ZombiesController zombies)
        {
            this.scope = scope; this.agent = agent; this.damageZone = damageZone; this.attackConfig = attackConfig;
            this.humans = humans; this.zombies = zombies;
        }

        public void Start()
        {
            zombies.Register(scope);
            scope.Hp.Died += OnDied;
        }

        public void Tick()
        {
            if (scope.Hp.IsDead) return;
            if (!IsValid(target)) target = humans.FindClosestAlive(scope.transform.position);
            if (target == null) { Stop(); return; }
            if (IsInDamageZone(target.BodyCollider))
            {
                Stop();
                if (Time.time >= nextAttackTime)
                {
                    if (attackConfig != null)
                    {
                        target.Hp.TakeDamage(attackConfig.Damage);
                        nextAttackTime = Time.time + attackConfig.IntervalSeconds;
                    }
                }
                return;
            }
            if (agent.isOnNavMesh) { agent.isStopped = false; agent.SetDestination(target.transform.position); }
        }

        public void Dispose() { scope.Hp.Died -= OnDied; zombies.Unregister(scope); }
        private void OnDied() { Stop(); zombies.Unregister(scope); Object.Destroy(scope.gameObject); }
        private bool IsValid(ManLifetimeScope value) => value != null && value.Hp != null && !value.Hp.IsDead && value.gameObject.activeInHierarchy;
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
