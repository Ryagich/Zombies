using UnityEngine;

namespace Zombies.StateMachine.Graph.Model
{
    public abstract class BaseBehaviour : ScriptableObject
    {
        public virtual void Enter(StateMachineContext context) { }
        public virtual void Logic(StateMachineContext context) { }
        public virtual void Exit(StateMachineContext context) { }
    }
}
