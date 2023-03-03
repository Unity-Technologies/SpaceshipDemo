using GameplayIngredients.StateMachines;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.stateMachinePath + "State Logic")]
    [Callable("State Machines", "Logic/ic-generic-logic.png")]
    public class StateLogic : LogicBase
    {
        [NonNullCheck]
        public StateMachine StateMachine;
        [NonNullCheck]
        public State TargetState;

        public Callable[] IfCurrentState;
        public Callable[] IfNotCurrentState;

        public override void Execute(GameObject instigator = null)
        {
            if (StateMachine?.CurrentState == TargetState && IfCurrentState != null && IfCurrentState.Length > 0)
                Call(IfCurrentState, instigator);
            else if (StateMachine?.CurrentState != TargetState && IfNotCurrentState != null && IfNotCurrentState.Length > 0)
                Call(IfNotCurrentState, instigator);
        }

        public override string GetDefaultName()
        {
            return $"On State Machine '{StateMachine?.gameObject.name}' state: '{TargetState?.gameObject.name}'";
        }
    }
}

