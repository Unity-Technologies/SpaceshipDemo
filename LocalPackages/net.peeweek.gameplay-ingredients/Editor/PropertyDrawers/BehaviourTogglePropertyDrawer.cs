using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(Actions.ToggleBehaviourAction.BehaviourToggle))]
    public class BehaviourTogglePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var toggle = property.FindPropertyRelative("State");
            var bhv = property.FindPropertyRelative("Behaviour");

            var toggleRect = new Rect(position);
            toggleRect.xMin = toggleRect.xMax - 80;

            var objRect = new Rect(position);
            objRect.xMax -= 80;

            toggle.intValue = EditorGUI.IntPopup(toggleRect, toggle.intValue, labels, values);
            bhv.objectReferenceValue = EditorGUI.ObjectField(objRect, bhv.objectReferenceValue, typeof(Behaviour), true);
        }

        static GUIContent[] labels = { new GUIContent("Disable"), new GUIContent("Enable"), new GUIContent("Toggle") };
        static int[] values = { 0, 1, 2 };
    }
}


