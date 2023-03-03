using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(UIEventManager), true)]
    public class UIEventManagerEditor : ManagerEditor
    {
        public override void OnInspectorGUI_PingArea()
        {
            base.OnInspectorGUI_PingArea();

#if ENABLE_INPUT_SYSTEM
            if(!(serializedObject.targetObject as UIEventManager).gameObject.TryGetComponent(out UnityEngine.InputSystem.UI.InputSystemUIInputModule issim))
            {
                using(new EditorGUI.IndentLevelScope(1))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Bad Configuration : New Input System Standalone Input", EditorStyles.boldLabel);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.HelpBox("You are using the new Input System, but the current game object is missing a InputSystemUIInputModule", MessageType.Warning);
                        if (GUILayout.Button("Fix", GUILayout.ExpandHeight(true), GUILayout.Width(80)))
                        {
                            (serializedObject.targetObject as UIEventManager).gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                        }
                    }
                }
            }
#endif

#if !ENABLE_LEGACY_INPUT_MANAGER
            if ((serializedObject.targetObject as UIEventManager).gameObject.TryGetComponent(out UnityEngine.EventSystems.StandaloneInputModule sim))
            {
                using (new EditorGUI.IndentLevelScope(1))
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Bad Configuration : Legacy Input System", EditorStyles.boldLabel);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.HelpBox("You are not using the Legacy Input System, but the current game object is hosting a StandaloneInputModule", MessageType.Warning);
                        if (GUILayout.Button("Remove", GUILayout.ExpandHeight(true), GUILayout.Width(80)))
                        {
                            DestroyImmediate((serializedObject.targetObject as UIEventManager).gameObject.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>());
                        }
                    }
                }
            }
#endif
        }

        static readonly Color color = new Color(.8f, .6f, .1f, 1f);
    }
}
