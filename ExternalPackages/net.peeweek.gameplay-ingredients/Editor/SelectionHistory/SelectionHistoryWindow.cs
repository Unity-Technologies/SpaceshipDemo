using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    class SelectionHistoryWindow : EditorWindow
    {
        public static SelectionHistoryWindow instance { get => s_Instance; }
        public static SelectionHistoryWindow s_Instance;

        [MenuItem("Window/General/Selection History &H")]
        public static void OpenSelectionHistoryWindow()
        {
            s_Instance = EditorWindow.GetWindow<SelectionHistoryWindow>();
        }

        private void OnDestroy()
        {
            s_Instance = null;
        }

        Vector2 scrollPos = Vector2.zero;

        void OnGUI()
        {

            titleContent = Contents.title;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                Selection_OnGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        public bool CompareArray(Object[] a, Object[] b)
        {
            return a.SequenceEqual(b);
        }

        void Selection_OnGUI()
        {
            if (History.selectionHistory == null) History.selectionHistory = new List<Object>();
            if (History.lockedObjects == null) History.lockedObjects = new List<Object>();
            int i = 0;
            int toRemove = -1;

            if (History.lockedObjects.Count > 0)
            {
                GUI.backgroundColor = new Color(0, 0, 0, 0.2f);
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    GUILayout.Label("Favorites", EditorStyles.boldLabel);
                }

                GUI.backgroundColor = Color.white;
                i = 0;
                toRemove = -1;
                foreach (var obj in History.lockedObjects)
                {
                    
                    if (obj == null)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.Label("(object is either null or has been deleted)");
                            if (GUILayout.Button("×", GUILayout.Width(16)))
                            {
                                toRemove = i;
                            }
                        }
                    }
                    else
                    {
                        bool highlight = Selection.objects.Contains(obj);
                        GUI.backgroundColor = highlight ? Styles.highlightColor : Color.white;

                        using (new EditorGUILayout.HorizontalScope(Styles.historyLine))
                        {
                            var b = GUI.color;
                            GUI.color = Color.yellow * 3;
                            if (GUILayout.Button(Contents.star, Styles.icon, GUILayout.Width(16)))
                            {
                                toRemove = i;
                            }

                            GUI.color = b;

                            GUIContent label = EditorGUIUtility.ObjectContent(obj, obj.GetType());
                            string name = label.text;
                            label.text = string.Empty;
                            GUILayout.Label(label, Styles.icon);
                            if (GUILayout.Button($"{name} ({obj.GetType().Name})", Styles.historyItem))
                            {
                                Selection.activeObject = obj;
                            }

                            if (obj is GameObject && !PrefabUtility.IsPartOfPrefabAsset(obj))
                            {
                                if (GUILayout.Button("Focus", Styles.historyButton, GUILayout.Width(48)))
                                {
                                    Selection.activeObject = obj;
                                    SceneView.lastActiveSceneView.FrameSelected();
                                }
                            }
                            else if (GUILayout.Button("Open", Styles.historyButton, GUILayout.Width(48)))
                            {
                                AssetDatabase.OpenAsset(obj);
                            }
                        }
                    }
                    i++;
                }
                if (toRemove != -1) History.lockedObjects.RemoveAt(toRemove);

                GUILayout.Space(8);
            }

            int toAdd = -1;
            toRemove = -1;
            i = 0;
            
            GUI.backgroundColor = new Color(0, 0, 0, 0.2f);
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("History", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                {
                    History.selectionHistory.Clear();
                    Repaint();
                }
            }



            GUI.backgroundColor = Color.white;
            var reversedHistory = History.selectionHistory.Reverse<Object>().ToArray();
            foreach (var obj in reversedHistory)
            {
                if (obj != null)
                {
                    bool highlight = Selection.objects.Contains(obj);
                    GUI.backgroundColor = highlight ? Styles.highlightColor : Color.white;      

                    using (new EditorGUILayout.HorizontalScope(Styles.historyLine))
                    {
                        bool favorited = History.lockedObjects.Contains(obj);

                        if (GUILayout.Button(favorited ? Contents.star : Contents.starDisabled, Styles.icon, GUILayout.Width(16)))
                        {
                            if (!favorited)
                                toAdd = i;
                            else
                                toRemove = i;
                        }

                        GUIContent label = EditorGUIUtility.ObjectContent(obj, obj.GetType());
                        string name = label.text;
                        label.text = string.Empty;
                        GUILayout.Label(label, Styles.icon);
                        if (GUILayout.Button($"{name} ({obj.GetType().Name})", Styles.historyItem))
                        {

                            Selection.activeObject = obj;
                        }

                        if (obj is GameObject && !PrefabUtility.IsPartOfPrefabAsset(obj))  
                        {
                            if (GUILayout.Button("Focus", Styles.historyButton, GUILayout.Width(48)))
                            {
                                Selection.activeObject = obj;
                                SceneView.lastActiveSceneView.FrameSelected();
                            }
                        }
                        else if (GUILayout.Button("Open", Styles.historyButton, GUILayout.Width(48)))
                        {
                            AssetDatabase.OpenAsset(obj);
                        }
                    }

                }

                i++;
            }
            if (toAdd != -1)
            {
                History.lockedObjects.Add(reversedHistory[toAdd]);
                Repaint();
            }
            if (toRemove != -1)
            {
                History.lockedObjects.Remove(reversedHistory[toRemove]);
                Repaint();
            }

        }

        [InitializeOnLoad]
        static class History
        {
            [SerializeField]
            internal static List<Object> selectionHistory;
            [SerializeField]
            internal static List<Object> lockedObjects;

            static History()
            {
                selectionHistory = new List<Object>();
                lockedObjects = new List<Object>();
                Selection.selectionChanged += OnSelectionChange;
            }

            static void OnSelectionChange()
            {
                if (selectionHistory == null) selectionHistory = new List<Object>();
                if (lockedObjects == null) lockedObjects = new List<Object>();

                if (Selection.activeObject != null || Selection.objects.Length > 0)
                {

                    foreach (var obj in Selection.objects)
                    {
                        if (!selectionHistory.Contains(obj))
                            selectionHistory.Add(obj);
                    }
                }

                if (SelectionHistoryWindow.instance != null)
                    SelectionHistoryWindow.instance.Repaint();
            }
        }

        static class Styles
        {
            public static GUIStyle historyLine;
            public static GUIStyle historyItem;
            public static GUIStyle historyButton;
            public static GUIStyle highlight;
            public static Color highlightColor = new Color(1.0f, 1.5f, 2.0f);

            public static GUIStyle icon;

            static Styles()
            {
                historyLine = new GUIStyle(EditorStyles.toolbarButton);

                historyItem = new GUIStyle(EditorStyles.foldout);
                historyItem.active.background = Texture2D.blackTexture;
                historyItem.onActive.background = Texture2D.blackTexture;
                historyItem.focused.background = Texture2D.blackTexture;
                historyItem.onFocused.background = Texture2D.blackTexture;
                historyItem.hover.background = Texture2D.blackTexture;
                historyItem.onHover.background = Texture2D.blackTexture;
                historyItem.normal.background = Texture2D.blackTexture;
                historyItem.onNormal.background = Texture2D.blackTexture;
                historyItem.fixedHeight = 16;
                historyItem.padding = new RectOffset();


                historyButton = new GUIStyle(EditorStyles.miniButton);
                historyButton.alignment = TextAnchor.MiddleLeft;

                highlight = new GUIStyle(EditorStyles.miniLabel);
                highlight.onNormal.background = Texture2D.whiteTexture;
                highlight.onHover.background = Texture2D.whiteTexture;
                highlight.onActive.background = Texture2D.whiteTexture;
                highlight.onFocused.background = Texture2D.whiteTexture;

                icon = new GUIStyle(EditorStyles.label);
                icon.fixedHeight = 16;
                icon.fixedWidth = 16;
                icon.padding = new RectOffset();
                icon.margin = new RectOffset(0,8,0,0);

            }
        }

        static class Contents
        {
            public static GUIContent title;
            public static GUIContent star = new GUIContent(EditorGUIUtility.IconContent("Favorite Icon").image);
            public static GUIContent starDisabled = new GUIContent(EditorGUIUtility.IconContent("Favorite").image);

            static Contents()
            {
                title = EditorGUIUtility.IconContent("EventTrigger Icon");
                title.text = "Selection History";
            }
        }
    }
}
