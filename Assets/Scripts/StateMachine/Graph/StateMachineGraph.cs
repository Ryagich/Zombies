using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zombies.StateMachine.Graph.Model;

namespace Zombies.StateMachine.Graph
{
    [CreateAssetMenu(fileName = "StateMachineGraph", menuName = "Zombies/State Machine/Graph")]
    public sealed class StateMachineGraph : ScriptableObject
    {
        public List<Node> Nodes = new();
        public State GetEntryState() => Nodes.FirstOrDefault(node => node?.State != null)?.State;
    }
}
