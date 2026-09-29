using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zombies.StateMachine.Graph.Model
{
    [CreateAssetMenu(fileName = "Transition", menuName = "Zombies/State Machine/Transition")]
    public sealed class Transition : ScriptableObject
    {
        public TransitionType Type = TransitionType.All;
        public List<BaseCondition> Conditions = new();
        public List<ActionOnTransitionBase> Actions = new();
        public State TargetState;

        public bool CanTransition(StateMachineContext context)
        {
            if (TargetState == null || Conditions == null) return false;
            return Type == TransitionType.All
                ? Conditions.All(condition => condition != null && condition.IsCondition(context))
                : Conditions.Any(condition => condition != null && condition.IsCondition(context));
        }
    }

    public enum TransitionType { Any, All }
}
