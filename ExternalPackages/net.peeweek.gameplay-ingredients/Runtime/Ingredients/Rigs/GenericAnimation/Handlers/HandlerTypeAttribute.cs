using System;
using UnityEngine;

namespace GameplayIngredients.Rigs
{
    public class HandlerTypeAttribute : PropertyAttribute
    {
        public Type type;
        public HandlerTypeAttribute(Type type)
        {
            this.type = type;
        }
    }
}

