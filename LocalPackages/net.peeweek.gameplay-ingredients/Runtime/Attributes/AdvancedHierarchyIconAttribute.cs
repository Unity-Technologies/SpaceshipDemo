using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AdvancedHierarchyIconAttribute : Attribute
    {
        public readonly string icon;

        public AdvancedHierarchyIconAttribute(string icon)
        {
            this.icon = icon;
        }
    }
}
