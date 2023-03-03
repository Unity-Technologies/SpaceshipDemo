using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes.Editor;
using GameplayIngredients.Rigs;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(GameplayIngredientsBehaviour), true)]
    public class GameplayIngredientsBehaviourEditor : IngredientEditor
    {
        public override void OnInspectorGUI_PingArea()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawBreadCrumb(ObjectNames.NicifyVariableName(serializedObject.targetObject.GetType().Name), color, () =>
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(serializedObject.targetObject.GetType().Name), GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
            });

            DrawBaseProperties();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        static readonly Color color = new Color(.1f, .8f, .6f, 1f);
    }
}
