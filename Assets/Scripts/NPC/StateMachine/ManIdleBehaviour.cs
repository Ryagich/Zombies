using UnityEngine;
using UnityEngine.AI;
using Zombies.StateMachine;
using Zombies.StateMachine.Graph.Model;

namespace Zombies.NPC.StateMachine
{
    public sealed class ManIdleBehaviour : BaseBehaviour
    {
        public override void Enter(StateMachineContext context)
        {
            var agent = context.GetService<NavMeshAgent>();
            if (agent == null)
                return;

            agent.isStopped = true;
            if (agent.isOnNavMesh)
                agent.ResetPath();
        }
    }
}
