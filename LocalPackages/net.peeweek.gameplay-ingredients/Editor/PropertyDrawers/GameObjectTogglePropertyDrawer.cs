using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(Actions.ToggleGameObjectAction.GameObjectToggle))]
    public class GameObjectTogglePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var toggle = property.FindPropertyRelative("State");
            var obj = property.FindPropertyRelative("GameObject");

            var toggleRect = new Rect(position);
            toggleRect.xMin = toggleRect.xMax - 80;

            var objRect = new Rect(position);
            objRect.xMax -= 80;

            toggle.intValue = EditorGUI.IntPopup(toggleRect, toggle.intValue, labels, values);
            obj.objectReferenceValue = EditorGUI.ObjectField(objRect, obj.objectReferenceValue, typeof(GameObject), true);
        }

        static GUIContent[] labels = { new GUIContent("Disable"), new GUIContent("Enable"), new GUIContent("Toggle") };
        static int[] values = { 0, 1, 2 };
    }
}


