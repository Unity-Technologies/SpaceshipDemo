using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Flip Flop Logic")]
    [Callable("Logic", "Logic/ic-generic-logic.png")]
    public class FlipFlopLogic : LogicBase
    {
        public enum State
        {
            Flip,
            Flop
        }

        public State InitialState = State.Flip;

        public Callable[] OnFlip;
        public Callable[] OnFlop;

        private State state;

        public void OnEnable()
        {
            state = InitialState;
        }

        public override void Execute(GameObject instigator = null)
        {
            if (state == State.Flop)
            {
                Callable.Call(OnFlip, instigator);
                state = State.Flip;
            }
            else
            {
                Callable.Call(OnFlop, instigator);
                state = State.Flop;
            }
        }

        public override string GetDefaultName()
        {
            return $"Flip-Flop (Default {state})";
        }
    }
}
