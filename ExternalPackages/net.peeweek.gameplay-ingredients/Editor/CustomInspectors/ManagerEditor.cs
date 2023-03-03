using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes.Editor;
using GameplayIngredients.Rigs;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(Manager), true)]
    public class ManagerEditor : IngredientEditor
    {
        public override void OnInspectorGUI_PingArea()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawBreadCrumb("Manager", color, () =>
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(serializedObject.targetObject.GetType().Name), GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
            });

            if(GameplayIngredientsSettings.currentSettings.excludedeManagers.Contains(serializedObject.targetObject.GetType().Name))
            {
                EditorGUILayout.HelpBox("This manager is currently excluded, please check the Manager Exclusion list in your GameplayIngredientsSettings asset located in Assets/Resources", MessageType.Warning);
            }

            DrawBaseProperties();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        static readonly Color color = new Color(.9f, .9f, .1f, 1f);
    }
}
