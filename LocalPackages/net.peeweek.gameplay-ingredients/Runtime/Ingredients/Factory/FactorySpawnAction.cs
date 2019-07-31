using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class FactorySpawnAction : ActionBase
    {
        [NonNullCheck]
        public Factory factory;

        public override void Execute(GameObject instigator = null)
        {
            if (factory != null)
                factory.Spawn();
        }
    }
}
