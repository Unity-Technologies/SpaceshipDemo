using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes.Editor;
using GameplayIngredients.Rigs;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(Rig), true)]
    public class RigEditor : IngredientEditor
    {
        SerializedProperty m_UpdateMode;
        SerializedProperty m_RigPriority;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_UpdateMode = serializedObject.FindProperty("m_UpdateMode");
            m_RigPriority = serializedObject.FindProperty("m_RigPriority");

        }

        public override void OnInspectorGUI_PingArea()
        {
            serializedObject.Update();

            bool excludedRigManager = GameplayIngredientsSettings.currentSettings.excludedeManagers.Any(s => s == "RigManager");

            if (excludedRigManager)
            {
                EditorGUILayout.HelpBox("This Rig depends on the Rig Manager which is excluded in your Gameplay Ingredients Settings. This rig component will be inactive unless the manager is not excluded.", MessageType.Error, true);
                if (GUILayout.Button("Open Settings"))
                    Selection.activeObject = GameplayIngredientsSettings.currentSettings;
            }

            EditorGUI.BeginDisabledGroup(excludedRigManager);

            EditorGUI.BeginChangeCheck();

            DrawBreadCrumb("Rig", color, () =>
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(serializedObject.targetObject.GetType().Name));
                GUILayout.FlexibleSpace();
                OpenIngredientsExplorerButton(serializedObject.targetObject as Rig);
            });

            using (new GUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(true)))
            {
                GUILayout.Label("Rig Update Properties", EditorStyles.boldLabel);
                using (new EditorGUI.IndentLevelScope(1))
                {
                    NaughtyEditorGUI.PropertyField_Layout(m_UpdateMode, true);
                    NaughtyEditorGUI.PropertyField_Layout(m_RigPriority, true);
                }
            }

            DrawBaseProperties();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndDisabledGroup();

        }

        static readonly Color color = new Color(1f, .5f, .1f, 1f);

    }
}
