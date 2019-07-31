using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightEventAnimation))]
public class LightEventAnimationEditor : Editor
{
    SerializedProperty animationManager;
    SerializedProperty animationMode;
    SerializedProperty animationLength;
    SerializedProperty curveSettings;
    SerializedProperty noiseSettings;
    SerializedProperty customSettings;
    SerializedProperty onAnimationEnd;

    LightEventAnimation targetObject;

    private void OnEnable()
    {
        animationManager = serializedObject.FindProperty("animationManager");
        animationMode = serializedObject.FindProperty("animationMode");
        animationLength = serializedObject.FindProperty("animationLength");
        curveSettings = serializedObject.FindProperty("curveSettings");
        noiseSettings = serializedObject.FindProperty("noiseSettings");
        customSettings = serializedObject.FindProperty("customSettings");
        onAnimationEnd = serializedObject.FindProperty("onAnimationEnd");

        targetObject = (LightEventAnimation)serializedObject.targetObject;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(animationManager);
        EditorGUILayout.PropertyField(animationMode);
        EditorGUILayout.PropertyField(animationLength);

        EditorGUILayout.BeginVertical("Box");
        if(animationMode.enumValueIndex == 0)
            EditorGUILayout.PropertyField(curveSettings);
        if (animationMode.enumValueIndex == 1)
            EditorGUILayout.PropertyField(noiseSettings);
        if (animationMode.enumValueIndex == 2)
            EditorGUILayout.PropertyField(customSettings);
        EditorGUILayout.EndVertical();

        EditorGUILayout.PropertyField(onAnimationEnd);

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(SwitchOffLightAnimation))]
public class SwitchOffLightAnimationEditor : LightEventAnimationEditor
{
}

[CustomEditor(typeof(SwitchOnLightAnimation))]
public class SwitchOnLightAnimationEditor : LightEventAnimationEditor
{
}

[CustomEditor(typeof(BreakLightAnimation))]
public class BreakLightAnimationEditor : LightEventAnimationEditor
{
}

[CustomEditor(typeof(SpecialEventLightAnimation))]
public class SpecialEventLightAnimationEditor : LightEventAnimationEditor
{
}

[CustomPropertyDrawer(typeof(CurveLightAnimationSettings))]
public class CurveLightAnimationSettingsPropertyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.LabelField("Curve settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("intensityCurve"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("loopAnimation"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }
}

[CustomPropertyDrawer(typeof(NoiseLightAnimationSettings))]
public class NoiseLightAnimationSettingsPropertyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.LabelField("Noise settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("frequency"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("minimumValue"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("maximumValue"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("jumpFrequency"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }
}

[CustomPropertyDrawer(typeof(CustomLightAnimationSettings))]
public class CustomLightAnimationSettingsPropertyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(property.FindPropertyRelative("value"));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 0;
    }
}
