using Zombies.StateMachine;
using Zombies.StateMachine.Graph.Model;

namespace Zombies.NPC.StateMachine
{
    public sealed class ManHasNoPatrolPointsCondition : BaseCondition
    {
        public override bool IsCondition(StateMachineContext context) => context.GetService<ManLifetimeScope>()?.CanPatrol != true;
    }
}
