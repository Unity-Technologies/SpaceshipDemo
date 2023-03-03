using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(NonNullCheckAttribute))]
    public class NonNullCheckPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.propertyType == SerializedPropertyType.ObjectReference)
            {
                bool valid = property.objectReferenceValue != null;
                var color = GUI.backgroundColor;
                GUI.backgroundColor = valid ? color : Color.red; 
                EditorGUI.ObjectField(position, property);
                GUI.backgroundColor = color;
            }
            else
            {
                EditorGUI.LabelField(position, "Use [NonNullCheck] attribute only on Object fields");
            }
        }

        static class Styles
        {
            public static GUIStyle errorfield;
            
            static Styles()
            {
                errorfield = new GUIStyle(EditorStyles.objectField);
            }
        }
    }
}


