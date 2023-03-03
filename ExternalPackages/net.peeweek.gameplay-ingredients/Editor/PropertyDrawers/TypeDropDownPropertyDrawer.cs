using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(TypeDropDownAttribute))]
    public class TypeDropDownPropertyDrawer : PropertyDrawer
    {
        Dictionary<string, List<string>> m_AssignableTypeNames;

        Type type;

        

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(type == null)
                type = ((TypeDropDownAttribute)attribute).m_BaseType;

            CacheType(type);
            string TypeName = type.FullName;

            int index = m_AssignableTypeNames[TypeName].IndexOf(property.stringValue);
            
            EditorGUI.BeginChangeCheck();
            int newVal = EditorGUI.Popup(position, index, m_AssignableTypeNames[TypeName].ToArray());
            if(EditorGUI.EndChangeCheck() && index != newVal)
            {
                property.stringValue = m_AssignableTypeNames[TypeName][newVal];
            }
        }

        void CacheType(Type baseType)
        {
            if (m_AssignableTypeNames == null)
            {
                m_AssignableTypeNames = new Dictionary<string, List<string>>();

                string key = baseType.FullName;

                if (!m_AssignableTypeNames.ContainsKey(key))
                    m_AssignableTypeNames.Add(key, new List<string>());

                foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach(var type in assembly.GetTypes())
                    {
                        if(baseType.IsAssignableFrom(type) && !type.IsAbstract)
                        {
                            m_AssignableTypeNames[key].Add(type.Name);
                        }
                    }
                }
            }
        }
    }

}
