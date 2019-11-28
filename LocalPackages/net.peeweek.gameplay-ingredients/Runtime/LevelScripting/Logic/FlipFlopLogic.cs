using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Logic
{
    public class FlipFlopLogic : LogicBase
    {
        public enum State
        {
            Flip,
            Flop
        }

        public State InitialState = State.Flip;

        [ReorderableList]
        public Callable[] OnFlip;

        [ReorderableList]
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
    }
}
