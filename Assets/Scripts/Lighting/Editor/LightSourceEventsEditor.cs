using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightSourceEvents))]
public class LightSourceEventsEditor : Editor
{
    SerializedProperty defaultState;
    SerializedProperty useRandomDelay;
    SerializedProperty maxRandomDelay;
    SerializedProperty bakedIndirectMultiplier;
    SerializedProperty eventsReceivers;

    LightSourceEvents targetObject;

    private void OnEnable()
    {
        defaultState = serializedObject.FindProperty("defaultState");
        useRandomDelay = serializedObject.FindProperty("useRandomDelay");
        maxRandomDelay = serializedObject.FindProperty("maxRandomDelay");
        bakedIndirectMultiplier = serializedObject.FindProperty("bakedIndirectMultiplier");
        eventsReceivers = serializedObject.FindProperty("eventsReceivers");

        targetObject = (LightSourceEvents)serializedObject.targetObject;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginVertical("Box");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(defaultState);
        EditorGUILayout.PropertyField(bakedIndirectMultiplier);
        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            targetObject.SetLightDimmer();
            targetObject.SetIndirectMultiplier();
        }

        EditorGUILayout.PropertyField(useRandomDelay);
        if(useRandomDelay.boolValue)
            EditorGUILayout.PropertyField(maxRandomDelay);

        EditorGUILayout.PropertyField(eventsReceivers, true);

        serializedObject.ApplyModifiedProperties();
    }
}