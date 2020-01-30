using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.VFX;
#else
using UnityEngine.Experimental.VFX;
#endif
using UnityEditor;
using GameplayIngredients.StateMachines;
using UnityEngine.Playables;

namespace GameplayIngredients.Editor
{
    [InitializeOnLoad]
    public static class HierarchyHints
    {
        const string kMenuPath = "Edit/Advanced Hierarchy View %.";
        public const int kMenuPriority = 230;

        [MenuItem(kMenuPath, priority = kMenuPriority, validate = false)]
        static void Toggle()
        {
            if (Active)
                Active = false;
            else
                Active = true;
        }

        [MenuItem(kMenuPath, priority = kMenuPriority, validate = true)]
        static bool ToggleCheck()
        {
            Menu.SetChecked(kMenuPath, Active);
            return SceneView.sceneViews.Count > 0;
        }


        static readonly string kPreferenceName = "GameplayIngredients.HierarchyHints";

        public static bool Active
        {
            get
            {
                return EditorPrefs.GetBool(kPreferenceName, false);
            }

            set
            {
                EditorPrefs.SetBool(kPreferenceName, value);
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
        }

        static HierarchyHints()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
        }

        static Dictionary<Type, string> s_Definitions = new Dictionary<Type, string>()
        {
            { typeof(Folder), "Folder Icon"},
            { typeof(MonoBehaviour), "cs Script Icon"},
            { typeof(Camera), "Camera Icon"},
            { typeof(MeshRenderer), "MeshRenderer Icon"},
            { typeof(SkinnedMeshRenderer), "SkinnedMeshRenderer Icon"},
            { typeof(BoxCollider), "BoxCollider Icon"},
            { typeof(SphereCollider), "SphereCollider Icon"},
            { typeof(CapsuleCollider), "CapsuleCollider Icon"},
            { typeof(MeshCollider), "MeshCollider Icon"},
            { typeof(AudioSource), "AudioSource Icon"},
            { typeof(Animation), "Animation Icon"},
            { typeof(Animator), "Animator Icon"},
            { typeof(PlayableDirector), "PlayableDirector Icon"},
            { typeof(Light), "Light Icon"},
            { typeof(LightProbeGroup), "LightProbeGroup Icon"},
            { typeof(LightProbeProxyVolume), "LightProbeProxyVolume Icon"},
            { typeof(ReflectionProbe), "ReflectionProbe Icon"},
            { typeof(VisualEffect), "VisualEffect Icon"},
            { typeof(ParticleSystem), "ParticleSystem Icon"},
            { typeof(Canvas), "Canvas Icon"},
            { typeof(Image), "Image Icon"},
            { typeof(Text), "Text Icon"},
            { typeof(Button), "Button Icon"},
            { typeof(StateMachine), "Packages/net.peeweek.gameplay-ingredients/Icons/Misc/ic-StateMachine.png"},
            { typeof(State), "Packages/net.peeweek.gameplay-ingredients/Icons/Misc/ic-State.png"},
        };

        static void HierarchyOnGUI(int instanceID, Rect selectionRect)
        {
            if (!Active) return;

            var fullRect = selectionRect;
#if UNITY_2019_3_OR_NEWER
            fullRect.xMin = 32;
#else
            fullRect.xMin = 16;
#endif
            fullRect.xMax = EditorGUIUtility.currentViewWidth;
            GameObject o = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (o == null) return;
            
            var c = GUI.color;

            bool isFolder = o.GetComponent<Folder>() != null;

            if(isFolder)
            {
                fullRect.xMin += 28 + 14 * GetObjectDepth(o.transform);
                fullRect.width = 16;
                EditorGUI.DrawRect(fullRect, EditorGUIUtility.isProSkin? Styles.proBackground : Styles.personalBackground);
                DrawIcon(fullRect, Contents.GetContent(typeof(Folder)), o.GetComponent<Folder>().Color);
            }
            else
            {
                if (o.isStatic)
                {
                    GUI.Label(fullRect, " S");
                    EditorGUI.DrawRect(fullRect, Colors.dimGray);
                }

                foreach (var type in s_Definitions.Keys)
                {
                    if (o.GetComponents(type).Length > 0) selectionRect = DrawIcon(selectionRect, Contents.GetContent(type), Color.white);
                }
            }
            GUI.color = c;
        }

        static int GetObjectDepth(Transform t, int depth=0)
        {
            if (t.parent == null)
                return depth;
            else
                return GetObjectDepth(t.parent, depth + 1);
        }

        
        static Rect DrawIcon(Rect rect, GUIContent content, Color color, int size = 16)
        {
            GUI.color = color;
            GUI.Label(rect, content, Styles.icon);
            rect.width = rect.width - size;
            return rect;
        }

        static class Contents
        {
            static Dictionary<Type, GUIContent> s_Icons = new Dictionary<Type, GUIContent>();

            public static void AddIcon(Type type, string IconName)
            {
                GUIContent icon;

                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(IconName);

                if (texture == null)
                    icon = EditorGUIUtility.IconContent(IconName);
                else
                    icon = new GUIContent(texture);

                s_Icons.Add(type, icon);
            }

            public static GUIContent GetContent(Type t)
            {
                if (!s_Icons.ContainsKey(t) && s_Definitions.ContainsKey(t))
                    AddIcon(t,s_Definitions[t]);

                return s_Icons[t];
            }
        }

        static class Colors
        {
            public static Color orange = new Color(1.0f, 0.7f, 0.1f);
            public static Color red = new Color(1.0f, 0.4f, 0.3f);
            public static Color yellow = new Color(0.8f, 1.0f, 0.1f);
            public static Color green = new Color(0.2f, 1.0f, 0.1f);
            public static Color blue = new Color(0.5f, 0.8f, 1.0f);
            public static Color violet = new Color(0.8f, 0.5f, 1.0f);
            public static Color purple = new Color(1.0f, 0.5f, 0.8f);
            public static Color dimGray = new Color(0.4f, 0.4f, 0.4f, 0.2f);
        }

        static class Styles
        {
            public static GUIStyle rightLabel;
            public static GUIStyle icon;

            public static Color proBackground = new Color(0.25f, 0.25f, 0.25f, 1.0f);
            public static Color personalBackground = new Color(0.75f, 0.75f, 0.75f, 1.0f);

            static Styles()
            {
                rightLabel = new GUIStyle(EditorStyles.label);
                rightLabel.alignment = TextAnchor.MiddleRight;
                rightLabel.normal.textColor = Color.white;
                rightLabel.onNormal.textColor = Color.white;

                rightLabel.active.textColor = Color.white;
                rightLabel.onActive.textColor = Color.white;

                icon = new GUIStyle(rightLabel);
                icon.padding = new RectOffset();
                icon.margin = new RectOffset();
            }
        }

    }
}
