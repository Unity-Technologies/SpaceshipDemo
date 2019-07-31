using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using GameplayIngredients.Pickup;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(PickupItem))]
    public class PickupItemEditor : UnityEditor.Editor
    {
        ReorderableList m_RList;
        SerializedProperty m_OnPickup;

        private void OnEnable()
        {
            m_RList = new ReorderableList(((PickupItem)serializedObject.targetObject).effects, typeof(PickupEffectBase), false, true, false, false);
            m_RList.drawElementCallback = DrawElement;
            m_OnPickup = serializedObject.FindProperty("OnPickup");
        }

        void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            GUI.Label(rect,string.Format("#{0} - {1}", index+1,  ObjectNames.NicifyVariableName(((PickupItem)serializedObject.targetObject).effects[index].GetType().Name)));
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(8);
            m_RList.DoLayoutList();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OnPickup,true);
            serializedObject.ApplyModifiedProperties();
        }

    }
}

