using UnityEngine;
using UnityEditor;
using GameplayIngredients.StateMachines;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(State), true)]
    public class StateEditor : IngredientEditor
    {
        public override void OnInspectorGUI_PingArea()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            string name = (serializedObject.targetObject as State).gameObject.name;

            DrawBreadCrumb("State", color, () =>
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(name));
                GUILayout.FlexibleSpace();
                OpenIngredientsExplorerButton(serializedObject.targetObject as StateMachine);
            });

            DrawBaseProperties();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        static readonly Color color = new Color(.5f, .1f, 1f, 1f);
    }
}
