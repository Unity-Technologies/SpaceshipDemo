using NaughtyAttributes.Editor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameplayIngredients.Editor
{
    public abstract class PingableEditor : NaughtyInspector
    {
        public bool needRepaint { get => m_pingValue > 0; }
        float m_pingValue;
        WarnDisabledModuleAttribute m_RequiredModule;

        static MonoBehaviour m_NextToPing;

        static Dictionary<MonoBehaviour, PingableEditor> trackedEditors;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (trackedEditors == null)
                trackedEditors = new Dictionary<MonoBehaviour, PingableEditor>();

            if (!trackedEditors.ContainsKey(serializedObject.targetObject as MonoBehaviour))
                trackedEditors.Add(serializedObject.targetObject as MonoBehaviour, this);

            m_RequiredModule = serializedObject.targetObject.GetType().GetCustomAttribute<WarnDisabledModuleAttribute>();
        }

        protected override void OnDisable()
        {
            if (serializedObject != null && serializedObject.targetObject != null)
            {
                if (trackedEditors.ContainsKey(serializedObject.targetObject as MonoBehaviour))
                    trackedEditors.Remove(serializedObject.targetObject as MonoBehaviour);
            }
            else // Delete or scene change
            {
                trackedEditors.Clear();
            }
            base.OnDisable();
        }

        public abstract void OnInspectorGUI_PingArea();

        public override void OnInspectorGUI()
        {
            if(m_RequiredModule != null)
            {
                EditorGUILayout.HelpBox($"This Script Requires the {m_RequiredModule.module} module : It will not execute until you enable the module in the {m_RequiredModule.fixLocation}.", MessageType.Warning);
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(m_RequiredModule != null);

            Rect r = EditorGUILayout.BeginVertical();
            UpdatePing(r);

            OnInspectorGUI_PingArea();

            EditorGUILayout.EndVertical();

            if (needRepaint)
                Repaint();

            EditorGUI.EndDisabledGroup();
        }

        public static void PingObject(MonoBehaviour r)
        {
            m_NextToPing = r;
            lastEditorTime = EditorApplication.timeSinceStartup;

            // Trigger a repaint if the editor is currently visible
            if (trackedEditors != null && trackedEditors.ContainsKey(r))
                trackedEditors[r].Repaint();
        }

        static double lastEditorTime;

        protected bool UpdatePing(Rect r)
        {
            if (m_NextToPing == serializedObject.targetObject as MonoBehaviour)
            {
                m_pingValue = 1;
                m_NextToPing = null;
            }

            if (m_pingValue <= 0)
                return false;

            r.yMin -= 2;
            r.xMin -= 14;
            r.width += 14;
            r.height += 6;
            EditorGUI.DrawRect(r, new Color(15f / 256, 128f / 256, 190f / 256, m_pingValue));

            double time = EditorApplication.timeSinceStartup;
            float dt = (float)(time - lastEditorTime);

            m_pingValue -= 2 * dt; // 2 is hardcoded, TODO: Make a preference out of it
            lastEditorTime = time;
            return true;

        }
    }
}

