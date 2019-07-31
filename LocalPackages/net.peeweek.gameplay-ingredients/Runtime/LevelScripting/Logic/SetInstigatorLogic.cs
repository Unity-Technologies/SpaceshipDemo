using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Logic
{
    public class SetInstigatorLogic : LogicBase
    {
        [ReorderableList]
        public Callable[] Next;

        public GameObject NewInstigator;

        public override void Execute(GameObject instigator = null)
        {
            Call(Next, NewInstigator);
        }
    }
}

