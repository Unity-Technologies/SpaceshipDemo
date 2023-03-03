using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using GameplayIngredients.Pickup;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(PickupItem))]
    public class PickupItemEditor : IngredientEditor
    {
        ReorderableList m_RList;

        protected override void OnEnable()
        {
            m_RList = new ReorderableList(((PickupItem)serializedObject.targetObject).effects, typeof(PickupEffectBase), false, true, false, false);
            m_RList.drawHeaderCallback = (r) => GUI.Label(r, "Pickup Effects");
            m_RList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                GUI.Label(rect, $"#{index} - {ObjectNames.NicifyVariableName(((PickupItem)serializedObject.targetObject).effects[index]?.GetType().Name)}");
            };
            base.OnEnable();
        }


        public override void OnInspectorGUI_PingArea()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            DrawBreadCrumb(ObjectNames.NicifyVariableName(serializedObject.targetObject.GetType().Name), color, () =>
            {
                GUILayout.Label(ObjectNames.NicifyVariableName(serializedObject.targetObject.GetType().Name), GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
            });

            EditorGUILayout.HelpBox("Add Effects to the Pickup Item by adding Pickup Effect Components to this Game Object", MessageType.Info);
            m_RList.DoLayoutList();


            DrawBaseProperties();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        static readonly Color color = new Color(1f, .3f, .3f, 1f);

    }
}

