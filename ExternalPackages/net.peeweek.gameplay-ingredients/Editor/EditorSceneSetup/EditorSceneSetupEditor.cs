using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(EditorSceneSetup))]
    public class EditorSceneSetupEditor : UnityEditor.Editor
    {
        ReorderableList m_List;

        SerializedProperty m_LoadedScenes;
        SerializedProperty m_ActiveScene;

        private void OnEnable()
        {
            m_ActiveScene = serializedObject.FindProperty("ActiveScene");
            m_LoadedScenes = serializedObject.FindProperty("LoadedScenes");

            m_List = new ReorderableList(serializedObject, m_LoadedScenes, true, true, true, true);
            m_List.drawElementCallback = OnDrawElement;
            m_List.drawHeaderCallback = OnDrawHeader;

        }

        private void OnDrawHeader(Rect rect)
        {
            GUI.Label(rect, "Scene List");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var toggleRect = rect;
            toggleRect.width = 16;
            toggleRect.yMin += 2;

            var sceneRect = rect;
            sceneRect.xMin += 24;
            sceneRect.xMax -= 80;
            sceneRect.yMin += 2;
            sceneRect.height = 16;

            var loadedRect = rect;
            loadedRect.xMin = rect.xMax - 80;
            loadedRect.yMin += 2;

            bool active = m_ActiveScene.intValue == index;
            bool newActive = GUI.Toggle(toggleRect, active, GUIContent.none);
            if(GUI.changed && newActive != active)
            {
                m_ActiveScene.intValue = index;
            }

            var sceneAsset = (SceneAsset)EditorGUI.ObjectField(sceneRect, m_LoadedScenes.GetArrayElementAtIndex(index).FindPropertyRelative("Scene").objectReferenceValue, typeof(SceneAsset), false);
            if (GUI.changed)
            {
                m_LoadedScenes.GetArrayElementAtIndex(index).FindPropertyRelative("Scene").objectReferenceValue = sceneAsset;
            }

            EditorGUI.BeginDisabledGroup(index == 0);
            int visible = m_LoadedScenes.GetArrayElementAtIndex(index).FindPropertyRelative("Loaded").boolValue ? 1 : 0;
            visible = EditorGUI.IntPopup(loadedRect, visible, kLoadedItems, kLoadedIndices);

            if(GUI.changed)
            {
                m_LoadedScenes.GetArrayElementAtIndex(index).FindPropertyRelative("Loaded").boolValue = visible == 1 ? true : false;
            } else if(index == 0)
            {
                m_LoadedScenes.GetArrayElementAtIndex(index).FindPropertyRelative("Loaded").boolValue = true;
            }
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }

        static readonly int[] kLoadedIndices = new int[2] { 0, 1 };
        static readonly GUIContent[] kLoadedItems = new GUIContent[2] { new GUIContent("Not Loaded"), new GUIContent("Loaded") };

        public override void OnInspectorGUI()
        {
            m_List.DoLayoutList();
        }
    }
}


