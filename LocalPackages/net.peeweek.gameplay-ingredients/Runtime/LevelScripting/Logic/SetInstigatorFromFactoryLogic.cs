using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    public class SetInstigatorFromFactoryLogic : LogicBase
    {
        [ReorderableList]
        public Callable[] Next;

        [NonNullCheck]
        public Factory Factory;
        public int FactoryIndex = 0;

        public override void Execute(GameObject instigator = null)
        {
            if(Factory != null)
            {
                GameObject instance = Factory.GetInstance(FactoryIndex);
                if (instance != null)
                    Call(Next, instance);
            }
        }
    }
}

