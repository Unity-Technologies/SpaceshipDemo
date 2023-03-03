using UnityEngine;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.logicPath + "Set Instigator Logic")]
    [Callable("Game", "Logic/ic-generic-logic.png")]
    public class SetInstigatorLogic : LogicBase
    {
        public Callable[] Next;

        public GameObject NewInstigator;

        public override void Execute(GameObject instigator = null)
        {
            Call(Next, NewInstigator);
        }

        public override string GetDefaultName()
        {
            return $"Set Instigator : '{NewInstigator?.gameObject.name}'";
        }
    }
}

