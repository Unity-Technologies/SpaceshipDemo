using System;
using UnityEngine;

namespace GameplayIngredients
{
    public class TypeDropDownAttribute : PropertyAttribute
    {
        public Type m_BaseType;

        public TypeDropDownAttribute(Type baseType)
        {
            m_BaseType = baseType;
        }
    }
}
