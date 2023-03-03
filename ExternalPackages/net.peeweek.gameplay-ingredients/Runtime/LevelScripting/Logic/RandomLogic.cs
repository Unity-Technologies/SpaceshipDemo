using UnityEngine;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Random Logic")]
    [Callable("Logic", "Logic/ic-generic-logic.png")]
    public class RandomLogic : LogicBase
    {
        public Callable[] RandomCalls;

        public override void Execute(GameObject instigator = null)
        {
            int r = Random.Range(0, RandomCalls.Length);
            Callable.Call(RandomCalls[r], instigator);
        }

        public override string GetDefaultName()
        {
            return $"Call One at Random...";
        }
    }
}

