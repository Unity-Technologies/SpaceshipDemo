using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(QualityDropDownAttribute))]
public class QualityDropDownEditor : PropertyDrawer
{
    int[] s_Qualities;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(s_Qualities == null || s_Qualities.Length != QualitySettings.names.Length)
        {
            int length = QualitySettings.names.Length;
            s_Qualities = new int[length];
            for (int i = 0; i < length; i++)
                s_Qualities[i] = i;
        }
        property.intValue = EditorGUI.IntPopup(position, "Quality", property.intValue, QualitySettings.names, s_Qualities);
    }
}
