#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;
using GameplayIngredients.Events;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(PlayerInputAction))]
    public class InputPlayerActionPropertyDrawer: PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 8;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var playerInput = property.FindPropertyRelative("playerInput");
            var path = property.FindPropertyRelative("path");

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, playerInput, new GUIContent("Player Input"));
            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight + 2;
            
            var paths = GetPaths(playerInput.objectReferenceValue as PlayerInput);
            int selected = paths.IndexOf(path.stringValue);
            selected = EditorGUI.Popup(position, "Action", selected, paths);
            if(GUI.changed)
            {
                if (selected >= 0)
                    path.stringValue = paths[selected];
                else
                    path.stringValue = string.Empty;
            }
            EditorGUI.indentLevel--;
        }

        string[] GetPaths(PlayerInput playerInput)
        {
            if (playerInput == null || playerInput.actions == null)
                return new string[0];

            List<string> paths = new List<string>();

            foreach(var map in playerInput.actions.actionMaps)
            {
                if (map == null) continue;
                foreach(var action in map.actions)
                {
                    if (action == null) continue;

                    paths.Add($"{map.name}{InputAssetAction.pathSeparator}{action.name}");
                }
            }

            return paths.ToArray();
        }
    }
}
#endif
