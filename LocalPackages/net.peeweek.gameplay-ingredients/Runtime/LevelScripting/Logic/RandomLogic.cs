using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameplayIngredients.Logic
{
    public class RandomLogic : LogicBase
    {
        [ReorderableList]
        public Callable[] RandomCalls;

        public override void Execute(GameObject instigator = null)
        {
            int r = Random.Range(0, RandomCalls.Length);
            Callable.Call(RandomCalls[r], instigator);
        }
    }
}

