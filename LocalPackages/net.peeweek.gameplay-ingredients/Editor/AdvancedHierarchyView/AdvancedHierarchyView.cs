using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEditor;
using GameplayIngredients.StateMachines;
using UnityEngine.Playables;
using System.Linq;
using System.Reflection;

namespace GameplayIngredients.Editor
{
    [InitializeOnLoad]
    public static class AdvancedHierarchyView
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

        static AdvancedHierarchyView()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
            InitializeTypes();
        }

        static void InitializeTypes()
        {
            RegisterComponentType( typeof(MonoBehaviour), "cs Script Icon");
            RegisterComponentType( typeof(Camera), "Camera Icon");
            RegisterComponentType( typeof(MeshRenderer), "MeshRenderer Icon");
            RegisterComponentType( typeof(SkinnedMeshRenderer), "SkinnedMeshRenderer Icon");
            RegisterComponentType( typeof(BoxCollider), "BoxCollider Icon");
            RegisterComponentType( typeof(SphereCollider), "SphereCollider Icon");
            RegisterComponentType( typeof(CapsuleCollider), "CapsuleCollider Icon");
            RegisterComponentType( typeof(MeshCollider), "MeshCollider Icon");
            RegisterComponentType( typeof(AudioSource), "AudioSource Icon");
            RegisterComponentType( typeof(Animation), "Animation Icon");
            RegisterComponentType( typeof(Animator), "Animator Icon");
            RegisterComponentType( typeof(PlayableDirector), "PlayableDirector Icon");
            RegisterComponentType( typeof(Light), "Light Icon");
            RegisterComponentType( typeof(LightProbeGroup), "LightProbeGroup Icon");
            RegisterComponentType( typeof(LightProbeProxyVolume), "LightProbeProxyVolume Icon");
            RegisterComponentType( typeof(ReflectionProbe), "ReflectionProbe Icon");
            RegisterComponentType( typeof(VisualEffect), "VisualEffect Icon");
            RegisterComponentType( typeof(ParticleSystem), "ParticleSystem Icon");
            RegisterComponentType( typeof(Canvas), "Canvas Icon");
            RegisterComponentType( typeof(Image), "Image Icon");
            RegisterComponentType( typeof(Text), "Text Icon");
            RegisterComponentType( typeof(Button), "Button Icon");
            RegisterComponentType( typeof(Folder), "Folder Icon");

            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach(var type in types)
                    {
                        if(type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsAbstract)
                        {
                            var attrib = type.GetCustomAttribute<AdvancedHierarchyIconAttribute>();
                            if(attrib != null)
                            {
                                RegisterComponentType(type, attrib.icon);
                            }
                        }
                    }
                }
                catch
                {
                    Debug.LogWarning("Could not load types from assembly:" + assembly.FullName);
                }
            }
        }

        public static void RegisterComponentType(Type t, string iconName)
        {
            if (s_Definitions == null)
                s_Definitions = new Dictionary<Type, string>();

            if (!s_Definitions.ContainsKey(t))
                s_Definitions.Add(t, iconName);
        }

        public static IEnumerable<Type> allTypes { get { return s_Definitions.Keys; } }
        static Dictionary<Type, string> s_Definitions = new Dictionary<Type, string>();

        static void HierarchyOnGUI(int instanceID, Rect selectionRect)
        {
            if (!Active) return;

            var fullRect = selectionRect;
            fullRect.xMin = 32;
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
                if (o.isStatic && AdvancedHierarchyPreferences.showStatic)
                {
                    GUI.Label(fullRect, " S");
                    EditorGUI.DrawRect(fullRect, Colors.dimGray);
                }

                foreach (var type in s_Definitions.Keys)
                {
                    if(AdvancedHierarchyPreferences.IsVisible(type) && o.GetComponents(type).Length > 0) 
                        selectionRect = DrawIcon(selectionRect, Contents.GetContent(type), Color.white);
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
