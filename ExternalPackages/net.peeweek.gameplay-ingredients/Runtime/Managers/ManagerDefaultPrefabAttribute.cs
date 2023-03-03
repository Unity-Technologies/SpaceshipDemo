using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameplayIngredients
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ManagerDefaultPrefabAttribute : Attribute
    {
        public string prefab { get => m_Prefab; }
        string m_Prefab;

        public ManagerDefaultPrefabAttribute(string prefabName)
        {
            m_Prefab = prefabName;
        }
    }
}

