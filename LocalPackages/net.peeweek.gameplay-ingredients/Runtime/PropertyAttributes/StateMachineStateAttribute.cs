using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    public class StateMachineStateAttribute : PropertyAttribute
    {
        public readonly string PropertyName;

        public StateMachineStateAttribute(string propertyName = "")
        {
            PropertyName = propertyName;
        }
    }
}
