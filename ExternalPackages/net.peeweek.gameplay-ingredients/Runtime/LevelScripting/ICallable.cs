using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    public interface ICallable
    {
        void Execute(GameObject instigator = null);
    }

}
