using UnityEngine;

namespace GameplayIngredients.Logic
{
    [AddComponentMenu(ComponentMenu.factoryPath + "Set Instigator From Factory Logic")]
    [Callable("Game", "Logic/ic-generic-logic.png")]
    public class SetInstigatorFromFactoryLogic : LogicBase
    {
        public Callable[] Next;

        [NonNullCheck]
        public Factory Factory;
        public int FactoryIndex = 0;
        public bool ContinueEvenIfNull = false;

        public override void Execute(GameObject instigator = null)
        {
            if(Factory != null)
            {
                GameObject instance = Factory.GetInstance(FactoryIndex);
                if(instance != null || ContinueEvenIfNull)
                    Call(Next, instance);
            }
        }

        public override string GetDefaultName()
        {
            return $"Set Instigator From Factory: '{Factory?.gameObject.name}' #{FactoryIndex}";
        }
    }
}

