using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(Callable))]
    public class CallablePropertyDrawer : PropertyDrawer
    {
        private static Dictionary<string, Callable> setNextObjectValue = new Dictionary<string, Callable>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string path = Binding.propertyToPath(property);
            if (setNextObjectValue.ContainsKey(path))
            {
                property.objectReferenceValue = setNextObjectValue[path];
                property.serializedObject.ApplyModifiedProperties();
                GUI.changed = true;
                setNextObjectValue.Remove(path);

                if(IngredientsExplorerWindow.visible)
                {
                    IngredientsExplorerWindow.Refresh();
                }
            }

            if(property.objectReferenceValue == null)
            {
                GUI.backgroundColor = Color.red;
                EditorGUI.DrawRect(position, new Color(1.0f,0,0,0.25f));
            }

            var pickRect = new Rect(position);
            pickRect.xMin = pickRect.xMax - 184;
            pickRect.xMax -= 30;

            var gotoRect = new Rect(position);
            gotoRect.xMin = gotoRect.xMax - 24;

            var objRect = new Rect(position);
            objRect.xMax -= 188;

            var obj = EditorGUI.ObjectField(objRect, property.objectReferenceValue, typeof(Callable), true);

            if (GUI.changed)
                property.objectReferenceValue = obj;


            if(property.objectReferenceValue != null)
            {
                if (GUI.Button(gotoRect, ">"))
                {
                    Selection.activeObject = property.objectReferenceValue;
                    PingableEditor.PingObject(Selection.activeObject as MonoBehaviour);
                }

                if (GUI.Button(pickRect, (property.objectReferenceValue as Callable).Name, EditorStyles.popup))
                {
                    ShowMenu(property);
                }
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                GUI.Label(pickRect, "No Callable Selected", EditorStyles.popup);
                EditorGUI.EndDisabledGroup();
            }
            
            GUI.backgroundColor = Color.white;
            
        }

        void ShowMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();
            var components = (property.objectReferenceValue as Callable).gameObject.GetComponents<Callable>();
            foreach(var component in components)
            {
                menu.AddItem(new GUIContent(component.GetType().Name + " - " + component.Name), component == property.objectReferenceValue, SetMenu, new Binding(property, component));
            }

            menu.ShowAsContext();
        }

        void SetMenu(object o)
        {
            Binding binding = o as Binding;
            if (setNextObjectValue.ContainsKey(binding.propertyPath))
                setNextObjectValue[binding.propertyPath] = binding.callable;
            else
                setNextObjectValue.Add(binding.propertyPath, binding.callable);

        }

        class Binding
        {
            public string propertyPath;
            public Callable callable;
            public Binding(SerializedProperty serializedProperty, Callable value)
            {
                propertyPath = propertyToPath(serializedProperty);
                callable = value;
            }

            public static string propertyToPath(SerializedProperty serializedProperty)
            {
                return $"{serializedProperty.serializedObject.targetObject.GetInstanceID()}:{serializedProperty.propertyPath}";
            }
        }
        
    }
}


