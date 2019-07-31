using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    public class Logic : LogicBase
    {
        [ReorderableList]
        public Callable[] Calls;

        public override void Execute(GameObject instigator = null)
        {
            Callable.Call(Calls, instigator);
        }
    }
}

