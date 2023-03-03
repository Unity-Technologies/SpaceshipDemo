using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(NoLabelAttribute))]
    public class NoLabelPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.labelWidth = 0;
            EditorGUI.PropertyField(position, property, GUIContent.none);
        }
    }
}

