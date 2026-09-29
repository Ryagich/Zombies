using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Zombies.StateMachine.Graph.Model
{
    [CreateAssetMenu(fileName = "State", menuName = "Zombies/State Machine/State")]
    public sealed class State : ScriptableObject
    {
        [field: SerializeField] public LocalizedString Name { get; private set; }
        public List<BaseBehaviour> Behaviours = new();
        public List<Transition> Transitions = new();
    }
}
