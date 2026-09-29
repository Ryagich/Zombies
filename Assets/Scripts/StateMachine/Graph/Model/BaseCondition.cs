using UnityEngine;

namespace Zombies.StateMachine.Graph.Model
{
    public abstract class BaseCondition : ScriptableObject
    {
        public virtual bool Enter(StateMachineContext context) => false;
        public virtual bool IsCondition(StateMachineContext context) => false;
    }
}
