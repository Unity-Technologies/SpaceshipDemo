using UnityEngine;
using UnityEditor;
using NaughtyAttributes.Editor;
using GameplayIngredients.Actions;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(ActionBase), true)]
    public class ActionBaseEditor : IngredientEditor
    {
        SerializedProperty m_Name;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Name = serializedObject.FindProperty("Name");
        }

        public override void OnInspectorGUI_PingArea()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawBreadCrumb("Action", color, () =>
            {
                using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                    NaughtyEditorGUI.PropertyField_Layout(m_Name, true);
                OpenIngredientsExplorerButton(serializedObject.targetObject as ActionBase);
            });

            DrawBaseProperties("Name");

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        static readonly Color color = new Color(.8f, .25f, .35f, 1f);
    }
}
