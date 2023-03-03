using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(CallableNameAttribute))]
    public class CallableNamePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.width -= 28;
            EditorGUIUtility.labelWidth = 0;
            EditorGUI.DelayedTextField(position, property, GUIContent.none);
            position.xMin += position.width + 4;
            position.width = 24;
            position.yMin -= 1;
            position.height += 1;
            if(GUI.Button(position,"···"))
            {
                var obj = property.serializedObject.targetObject as Callable;
                if(obj != null)
                {
                    property.stringValue = obj.GetDefaultName();
                    GUI.changed = true;
                }
            }
        }
    }
}

