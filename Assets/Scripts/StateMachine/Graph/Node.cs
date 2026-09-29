using System;
using UnityEngine;
using Zombies.StateMachine.Graph.Model;

namespace Zombies.StateMachine.Graph
{
    [Serializable]
    public sealed class Node
    {
        public Vector2 Position;
        public State State;
        public Node(State state) => State = state;
    }
}
