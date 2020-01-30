using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.VFX;
#else
using UnityEngine.Experimental.VFX;
#endif
using UnityEngine.Timeline;

namespace GameplayIngredients.Editor
{
    [CustomEditor(typeof(Discover))]
    public class DiscoverEditor : UnityEditor.Editor
    {
        const string kEditPreferenceName = "GameplayIngredients.DiscoverEditor.Editing";

        static bool editing
        {
            get { return EditorPrefs.GetBool(kEditPreferenceName, false); }
            set { if (value != editing) EditorPrefs.SetBool(kEditPreferenceName, value); }
        }

        Discover m_Discover;

        private void OnEnable()
        {
            m_Discover = serializedObject.targetObject as Discover;
            if (m_Discover.transform.hideFlags != HideFlags.HideInInspector)
                m_Discover.transform.hideFlags = HideFlags.HideInInspector;
        }

        public override void OnInspectorGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Align Discover to View"))
                {
                    var transform = (serializedObject.targetObject as Discover).gameObject.transform;
                    var svTransform = SceneView.lastActiveSceneView.camera.transform;

                    transform.position = svTransform.position;
                    transform.rotation = svTransform.rotation;
                    transform.localScale = Vector3.one;
                }
                
                GUILayout.FlexibleSpace();
                editing = GUILayout.Toggle(editing, "Edit", EditorStyles.miniButton, GUILayout.Width(48));
            }

            if (editing)
                DrawDefaultInspector();
            else
                DrawDiscoverContentGUI(m_Discover);
        }

        public static void DrawDiscoverContentGUI(Discover discover)
        {
            if(!string.IsNullOrEmpty(discover.Category))
                GUILayout.Label(discover.Category, DiscoverWindow.Styles.subHeader);

            GUILayout.Label(discover.Name, DiscoverWindow.Styles.header);

            using (new GUILayout.VerticalScope(DiscoverWindow.Styles.indent))
            {
                if (discover.Description != null && discover.Description != string.Empty)
                {
                    GUILayout.Label(discover.Description, DiscoverWindow.Styles.body);
                }

                GUILayout.Space(8);

                foreach (var section in discover.Sections)
                {
                    SectionGUI(section);
                    GUILayout.Space(16);
                }
            }
        }

        public static void SectionGUI(DiscoverSection section)
        {
            using (new DiscoverWindow.GroupLabelScope(section.SectionName))
            {
                using (new GUILayout.VerticalScope(DiscoverWindow.Styles.slightIndent))
                {
                    GUILayout.Label(section.SectionContent, DiscoverWindow.Styles.body);

                    if (section.Actions != null && section.Actions.Length > 0)
                    {
                        GUILayout.Space(8);

                        using (new GUILayout.VerticalScope(GUI.skin.box))
                        {
                            foreach (var action in section.Actions)
                            {
                                using (new GUILayout.HorizontalScope())
                                {
                                    GUILayout.Label(action.Description);
                                    GUILayout.FlexibleSpace();
                                    using (new GUILayout.HorizontalScope(GUILayout.MinWidth(160), GUILayout.Height(22)))
                                    {
                                        ActionButtonGUI(action.Target);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void ActionButtonGUI(UnityEngine.Object target)
        {
            if (target == null)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Button("(No Object)");
                EditorGUI.EndDisabledGroup();
                return;
            }


            Type t = target.GetType();

            if (t == typeof(GameObject))
            {
                GameObject go = target as GameObject;
                
                if (GUILayout.Button("  Select  ", DiscoverWindow.Styles.buttonLeft))
                {
                    Selection.activeObject = go;
                }

                if(PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.NotAPrefab)
                {
                    if (GUILayout.Button("  Go to  ", DiscoverWindow.Styles.buttonRight))
                    {

                        Selection.activeObject = go;
                        SceneView.lastActiveSceneView.FrameSelected();
                    }
                }
                else
                {
                    if (GUILayout.Button("  Open  ", DiscoverWindow.Styles.buttonRight))
                    {
                        AssetDatabase.OpenAsset(go);
                    }
                }
            }
            else if (t == typeof(Discover))
            {
                if (GUILayout.Button("Discover"))
                {
                    var discover = target as Discover;
                    Selection.activeGameObject = discover.gameObject;
                    DiscoverWindow.SelectDiscover(discover);
                }
            }
            else if (t == typeof(VisualEffectAsset))
            {
                if (GUILayout.Button("Open VFX Graph"))
                {
                    VisualEffectAsset graph = target as VisualEffectAsset;
                    AssetDatabase.OpenAsset(graph);
                }
            }
            else if (t == typeof(Animation))
            {
                if (GUILayout.Button("Open Animation"))
                {
                    Animation animation = target as Animation;
                    AssetDatabase.OpenAsset(animation);
                }
            }
            else if (t == typeof(TimelineAsset))
            {
                if (GUILayout.Button("Open Timeline"))
                {
                    TimelineAsset timeline = target as TimelineAsset;
                    AssetDatabase.OpenAsset(timeline);
                }
            }
            else if (t == typeof(PlayableDirector))
            {
                if (GUILayout.Button("Open Director"))
                {
                    PlayableDirector director = target as PlayableDirector;

                    AssetDatabase.OpenAsset(director.playableAsset);
                    Selection.activeObject = director.gameObject;
                }
            }
            else if (t == typeof(Shader))
            {
                if (GUILayout.Button("Open Shader"))
                {
                    Shader shader = target as Shader;
                    AssetDatabase.OpenAsset(shader);
                }
            }
            else
            {
                if (GUILayout.Button("Select"))
                {
                    Selection.activeObject = target;
                }
            }
        }
    }
}

