using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

namespace GameplayIngredients.Editor
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class ScenePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int count = EditorSceneManager.sceneCountInBuildSettings;
            string[] sceneNames = new string[count];
            GUIContent[] displayedOptions = new GUIContent[count];
            int[] values = new int[count];
            string currentValue = property.stringValue;
            int selectedIndex = -1;

            for (int i = 0; i < sceneNames.Length; i++)
            {
                sceneNames[i] = SceneUtility.GetScenePathByBuildIndex(i).Split(new char[] { '\\', '/' }).Last().Replace(".unity", "");
                displayedOptions[i] = new GUIContent(sceneNames[i]);
                values[i] = i;

                if (currentValue == sceneNames[i])
                    selectedIndex = i;
            }

            int newVal = EditorGUI.IntPopup(position, new GUIContent(property.displayName), selectedIndex, displayedOptions, values);
            if (GUI.changed)
            {
                property.stringValue = sceneNames[newVal];
            }
        } 
    }
}


