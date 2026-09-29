using UnityEngine;
using UnityEngine.AI;
using Zombies.StateMachine;
using Zombies.StateMachine.Graph.Model;

namespace Zombies.NPC.StateMachine
{
    public sealed class ManPatrolBehaviour : BaseBehaviour
    {
        private const float ArrivalTolerance = 0.05f;

        private string IndexKey => $"{GetInstanceID()}.PatrolIndex";
        private string WaitKey => $"{GetInstanceID()}.PatrolWait";

        public override void Enter(StateMachineContext context)
        {
            context.SetValue(IndexKey, 0);
            context.SetValue(WaitKey, 0f);
            MoveToCurrentPoint(context);
        }

        public override void Logic(StateMachineContext context)
        {
            var scope = context.GetService<ManLifetimeScope>();
            var agent = context.GetService<NavMeshAgent>();
            if (scope == null || !scope.CanPatrol || agent == null || !agent.isOnNavMesh)
                return;

            context.TryGetValue(WaitKey, out float remainingWait);
            if (remainingWait > 0f)
            {
                remainingWait -= context.DeltaTime;
                context.SetValue(WaitKey, remainingWait);
                if (remainingWait > 0f)
                    return;

                context.TryGetValue(IndexKey, out int pointIndex);
                context.SetValue(IndexKey, pointIndex + 1);
                MoveToCurrentPoint(context);
                return;
            }

            if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + ArrivalTolerance)
                return;

            agent.isStopped = true;
            var config = context.GetService<ManPatrolConfig>();
            context.SetValue(WaitKey, config != null ? config.WaitAtPointSeconds : 0f);
        }

        public override void Exit(StateMachineContext context)
        {
            context.RemoveValue(IndexKey);
            context.RemoveValue(WaitKey);
        }

        private void MoveToCurrentPoint(StateMachineContext context)
        {
            var scope = context.GetService<ManLifetimeScope>();
            var agent = context.GetService<NavMeshAgent>();
            if (scope == null || agent == null || !agent.isOnNavMesh)
                return;

            context.TryGetValue(IndexKey, out int pointIndex);
            var point = scope.GetPatrolPoint(pointIndex);
            if (point == null)
                return;

            agent.isStopped = false;
            agent.SetDestination(point.position);
        }
    }
}
