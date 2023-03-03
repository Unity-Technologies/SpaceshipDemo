using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(ExcludedManagerAttribute))]
    public class ExcludedManagerPropertyDrawer : PropertyDrawer
    {
        List<string> cachedTypes;
        bool needRecache = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (cachedTypes == null || needRecache)
                cachedTypes = CacheTypes();

            string curValue = property.stringValue;

            int index = cachedTypes.IndexOf(curValue);
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(position, index, cachedTypes.ToArray());
            if (EditorGUI.EndChangeCheck() && index != newIndex)
            {
                property.stringValue = cachedTypes[newIndex];
                needRecache = true;
            }
        }

        List<string> CacheTypes()
        {
            var types = new List<string>();

            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(var type in assembly.GetTypes())
                {
                    if(typeof(Manager).IsAssignableFrom(type) 
                        && !type.IsAbstract 
                        && !type.CustomAttributes.Any(o => o.AttributeType == typeof(NonExcludeableManager)))
                    {
                        types.Add(type.Name);
                    }
                }
            }
            needRecache = false;
            return types;
            
        }
    }

}
