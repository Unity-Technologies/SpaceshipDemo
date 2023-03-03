using UnityEngine;
using UnityEditor;

namespace GameplayIngredients
{
    static class GUIUtils
    {
        public static void ColoredLabel(string text, Color color)
        {
            GUIContent label = new GUIContent(text);
            Rect r = GUILayoutUtility.GetRect(label, Styles.coloredLabel);
            EditorGUI.DrawRect(r, color);
            var r2 = new RectOffset(1, 1, 1, 1).Remove(r);
            EditorGUI.DrawRect(r2, color * new Color(.5f, .5f, .5f, 1f));
            GUI.contentColor = color * 2;
            GUI.Label(r, label, Styles.coloredLabel);
            GUI.contentColor = Color.white;
        }

        static class Styles
        {
            public static GUIStyle coloredLabel;
            static Styles()
            {
                coloredLabel = new GUIStyle(EditorStyles.label);
                coloredLabel.fontStyle = FontStyle.Bold;
                coloredLabel.fontSize = 10;
                coloredLabel.padding = new RectOffset(6, 6, 2, 2);
            }
        }
    }
}


