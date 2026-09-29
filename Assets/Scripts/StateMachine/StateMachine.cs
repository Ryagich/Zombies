using Zombies.StateMachine.Graph.Model;

namespace Zombies.StateMachine
{
    public sealed class StateMachine
    {
        private readonly Graph.StateMachineGraph graph;
        private readonly StateMachineContext context;
        public State CurrentState { get; private set; }

        public StateMachine(Graph.StateMachineGraph graph, StateMachineContext context = null)
        {
            this.graph = graph;
            this.context = context ?? new StateMachineContext();
        }

        public void Start() => SetState(graph?.GetEntryState());

        public void Tick(float deltaTime)
        {
            if (CurrentState == null) return;
            context.DeltaTime = deltaTime;
            context.ElapsedTime += deltaTime;
            foreach (var behaviour in CurrentState.Behaviours) behaviour?.Logic(context);
            foreach (var transition in CurrentState.Transitions)
            {
                if (transition == null || !transition.CanTransition(context)) continue;
                foreach (var action in transition.Actions) action?.DoAction(context);
                SetState(transition.TargetState);
                return;
            }
        }

        public void SetState(State nextState)
        {
            if (CurrentState == nextState) return;
            if (CurrentState != null)
                foreach (var behaviour in CurrentState.Behaviours) behaviour?.Exit(context);
            CurrentState = nextState;
            if (CurrentState == null) return;
            foreach (var behaviour in CurrentState.Behaviours) behaviour?.Enter(context);
            foreach (var transition in CurrentState.Transitions)
                if (transition != null)
                    foreach (var condition in transition.Conditions) condition?.Enter(context);
        }
    }
}
