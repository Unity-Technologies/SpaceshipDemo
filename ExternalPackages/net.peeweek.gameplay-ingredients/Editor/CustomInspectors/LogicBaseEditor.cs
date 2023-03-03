using UnityEngine;
using UnityEditor;
using NaughtyAttributes.Editor;
using GameplayIngredients.Logic;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(LogicBase), true)]
    public class LogicBaseEditor : IngredientEditor
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

            DrawBreadCrumb("Logic", color, () =>
            {
                NaughtyEditorGUI.PropertyField_Layout(m_Name, true);
                OpenIngredientsExplorerButton(serializedObject.targetObject as LogicBase);
            });

            DrawBaseProperties("Name");

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        static readonly Color color = new Color(.2f, .8f, .1f, 1f);
    }
}
