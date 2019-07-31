using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SubtitleManager.Subtitle))]
public class SubtitlePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var time = property.FindPropertyRelative("Time");
        var text = property.FindPropertyRelative("Text");

        var timeRect = new Rect(position);
        timeRect.xMin = timeRect.xMax - 80;

        var textRect = new Rect(position);
        textRect.xMax -= 80;

        time.floatValue = EditorGUI.FloatField(timeRect, time.floatValue);
        text.stringValue = EditorGUI.DelayedTextField(textRect, text.stringValue);
    }
}

