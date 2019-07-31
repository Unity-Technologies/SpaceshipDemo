using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace GameplayIngredients.Actions
{
    public class FactorySetTargetAction : ActionBase
    {
        [NonNullCheck]
        public Factory factory;

        [NonNullCheck]
        public GameObject Target;

        public override void Execute(GameObject instigator = null)
        {
            if (factory != null && Target != null)
            {
                factory.SetTarget(Target);
            }
        }
    }
}
