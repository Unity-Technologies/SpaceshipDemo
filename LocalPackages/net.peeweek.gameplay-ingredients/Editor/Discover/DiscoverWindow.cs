using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Experimental.VFX;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Timeline;

namespace GameplayIngredients.Editor
{
    public class DiscoverWindow : EditorWindow
    {
        static List<DiscoverAsset> s_StartupDiscoverAssets;

        static bool GetShowOnStartup(string name)
        {
            return EditorPrefs.GetBool($"{name}.ShowAtStartup", true);
        }

        static void SetShowOnStartup(string name, bool value)
        {
            if (value != GetShowOnStartup(name)) EditorPrefs.SetBool($"{name}.ShowAtStartup", value);
        }

        public static void SelectDiscover(Discover discover)
        {
            foreach(var window in s_Windows)
            {
                foreach(var categoryKvp in window.discoverObjects)
                {
                    if (categoryKvp.Value.Contains(discover))
                    {
                        window.SetSelectedDiscover(discover);
                        break;
                    }
                }
            }
        }

        public static void Reload()
        {
            EditorApplication.update -= ShowAtStartup;
            s_StartupDiscoverAssets = null;
            InitShowAtStartup();
        }

        [InitializeOnLoadMethod]
        static void InitShowAtStartup()
        {
            string[] guids = AssetDatabase.FindAssets("t:DiscoverAsset");
            foreach (var guid in guids)
            {
                DiscoverAsset asset = AssetDatabase.LoadAssetAtPath<DiscoverAsset>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset.EnableShowAtStartup)
                {
                    if (s_StartupDiscoverAssets == null)
                        s_StartupDiscoverAssets = new List<DiscoverAsset>();

                    s_StartupDiscoverAssets.Add(asset);
                }
            }

            if (s_StartupDiscoverAssets != null && s_StartupDiscoverAssets.Count > 0)
                EditorApplication.update += ShowAtStartup;
        }

        static void ShowAtStartup()
        {
            if (!Application.isPlaying && s_StartupDiscoverAssets != null)
            {
                foreach (var discoverAsset in s_StartupDiscoverAssets)
                {
                    if (GetShowOnStartup(discoverAsset.PreferenceName))
                        ShowDiscoverWindow(discoverAsset);
                }
            }
            EditorApplication.update -= ShowAtStartup;
        }

        static List<DiscoverWindow> s_Windows;

        public static void ShowDiscoverWindow(DiscoverAsset discoverAsset)
        {
            if (discoverAsset != null)
            {
                var window = GetWindow<DiscoverWindow>(!discoverAsset.dockable);
                window.SetDiscoverAsset(discoverAsset);
            }
            else
            {
                Debug.LogError("Could not open Discover Window : discoverAsset is null");
            }
        }

        [SerializeField]
        public DiscoverAsset discoverAsset { get; private set; }
        Texture2D header;
        bool forceGlobal;

        void SetDiscoverAsset(DiscoverAsset discover)
        {
            discoverAsset = discover;
            titleContent = new GUIContent(discoverAsset.WindowTitle);
            minSize = new Vector2(discoverAsset.WindowWidth, discoverAsset.WindowHeight);
            maxSize = new Vector2(discoverAsset.WindowWidth, discoverAsset.WindowHeight);
            UpdateDiscoverObjects();
        }

        private void OnEnable()
        {
            EditorSceneManager.newSceneCreated += UpdateDiscoverObjectsOnCreate;
            EditorSceneManager.sceneOpened += UpdateDiscoverObjectsOnLoad;
            EditorSceneSetup.onSetupLoaded += UpdateDiscoverObjectsOnLoadSetup;

            if (s_Windows == null)
                s_Windows = new List<DiscoverWindow>();

            if(!s_Windows.Contains(this))
                s_Windows.Add(this);
        }

        private void OnDisable()
        {
            EditorSceneManager.newSceneCreated -= UpdateDiscoverObjectsOnCreate;
            EditorSceneManager.sceneOpened -= UpdateDiscoverObjectsOnLoad;
            EditorSceneSetup.onSetupLoaded -= UpdateDiscoverObjectsOnLoadSetup;

            if (s_Windows.Contains(this))
                s_Windows.Remove(this);
        }

        Dictionary<string, List<Discover>> discoverObjects = null;

        void UpdateDiscoverObjectsOnLoadSetup(EditorSceneSetup setup)
        {
            forceGlobal = false;
            UpdateDiscoverObjects();
        }

        void UpdateDiscoverObjectsOnCreate(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            forceGlobal = false;
            UpdateDiscoverObjects();
        }

        void UpdateDiscoverObjectsOnLoad(Scene s, OpenSceneMode s2)
        {
            forceGlobal = false;
            UpdateDiscoverObjects();
        }


        /// <summary>
        /// Filters object based on asset tags, object tags and FilterMode
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="discoverObject"></param>
        /// <returns>true to discard, false to keep</returns>
        static bool FilterDiscoverObject(DiscoverAsset asset, Discover discoverObject)
        {
            if (string.IsNullOrEmpty(asset.Tags))
                asset.Tags = string.Empty;
            if (string.IsNullOrEmpty(discoverObject.Tags))
                discoverObject.Tags = string.Empty;

            string[] assetTags = asset.Tags.Split(' ');
            string[] objectTags = discoverObject.Tags.Split(' ');

            switch(asset.filterMode)
            {
                case DiscoverAsset.FilterMode.ShowAll: return false;
                case DiscoverAsset.FilterMode.IncludeTags:
                    return !assetTags.Intersect(objectTags).Any();
                case DiscoverAsset.FilterMode.ExcludeTags:
                    return assetTags.Intersect(objectTags).Any();
            }
            return false;
        }

        void UpdateDiscoverObjects(bool clear = false)
        {
            if (discoverObjects == null)
                discoverObjects = new Dictionary<string, List<Discover>>();

            if (clear)
                discoverObjects.Clear();

            Discover[] newOnes = FindObjectsOfType<Discover>();

            // Add new ones
            foreach (var item in newOnes)
            {
                // Apply Filter
                if (FilterDiscoverObject(discoverAsset, item))
                    continue;

                if (!discoverObjects.ContainsKey(item.Category))
                {
                    discoverObjects.Add(item.Category, new List<Discover>());
                }

                if (!discoverObjects[item.Category].Contains(item))
                {
                    discoverObjects[item.Category].Add(item);
                }
            }

            // Cleanup Empty Entries
            Dictionary<string, List<Discover>> cleanedUpLists = new Dictionary<string, List<Discover>>();

            foreach (var categoryKvp in discoverObjects)
            {
                cleanedUpLists.Add(categoryKvp.Key, categoryKvp.Value.Where((o) => o != null).ToList());
            }
            foreach (var categoryKvp in cleanedUpLists)
            {
                discoverObjects[categoryKvp.Key] = categoryKvp.Value;
            }

            // Cleanup Empty Categories
            List<string> toDelete = new List<string>();
            foreach (var categoryKvp in discoverObjects)
            {
                if (categoryKvp.Value == null || categoryKvp.Value.Count == 0)
                    toDelete.Add(categoryKvp.Key);
            }
            foreach (var category in toDelete)
            {
                discoverObjects.Remove(category);
            }

            // Finally, sort items in each category
            foreach (var categoryKvp in discoverObjects)
            {
                discoverObjects[categoryKvp.Key].Sort((a, b) => { return Comparer<int>.Default.Compare(a.Priority, b.Priority); });
            }

            // Ensure something is selected is possible

            if (selectedDiscover == null) // Try Fetching a default
            {
                foreach (var categoryKvp in discoverObjects)
                {
                    selectedDiscover = categoryKvp.Value.FirstOrDefault(o => o.DefaultSelected == true);
                    if (selectedDiscover != null)
                        break;
                }
            }
            if (selectedDiscover == null && discoverObjects != null && discoverObjects.Count > 0)
            {
                selectedDiscover = discoverObjects.First().Value.First();
            }
            
            Repaint();
        }

        private void OnGUI()
        {
            // Draw Header Image
            if (discoverAsset.HeaderTexture != null)
            {
                if (header == null || header != discoverAsset.HeaderTexture)
                    header = discoverAsset.HeaderTexture;

                Rect headerRect = GUILayoutUtility.GetRect(header.width, header.height);
                GUI.DrawTexture(headerRect, header);
            }
            else
            {
                Rect headerRect = GUILayoutUtility.GetRect(discoverAsset.WindowWidth, 80);
                EditorGUI.DrawRect(headerRect, new Color(0,0,0,0.2f));
                headerRect.xMin += 16;
                headerRect.yMin += 16;
                GUI.Label(headerRect, discoverAsset.WindowTitle, Styles.header);
            }

            bool hasContent = discoverObjects != null && discoverObjects.Count > 0;

            // Draw Navigation Bar
            EditorGUI.BeginDisabledGroup(!hasContent);
            using (new GUILayout.AreaScope(new Rect(discoverAsset.WindowWidth - 168, 8, 160, 20)))
            {
                using (new GUILayout.HorizontalScope(Styles.tabContainer))
                {
                    bool value = forceGlobal;
                    EditorGUI.BeginChangeCheck();
                    value = GUILayout.Toggle(forceGlobal || !hasContent, "Levels", Styles.buttonLeft);
                    if (EditorGUI.EndChangeCheck())
                    {
                        forceGlobal = true;
                    }

                    EditorGUI.BeginChangeCheck();
                    value = GUILayout.Toggle(!forceGlobal && hasContent, "Discover", Styles.buttonRight);
                    if (EditorGUI.EndChangeCheck())
                    {
                        forceGlobal = false;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            // Draw Content
            if (!hasContent || forceGlobal)
                GlobalContentGUI();
            else
                SceneContentGUI();

            // Draw Footer
            Rect line = GUILayoutUtility.GetRect(discoverAsset.WindowWidth, 1);
            EditorGUI.DrawRect(line, Color.black);
            using (new GUILayout.HorizontalScope())
            {
                if(discoverAsset.EnableShowAtStartup)
                {
                    EditorGUI.BeginChangeCheck();
                    bool showOnStartup = GUILayout.Toggle(GetShowOnStartup(discoverAsset.PreferenceName), " Show this window on startup");
                    if (EditorGUI.EndChangeCheck())
                    {
                        SetShowOnStartup(discoverAsset.PreferenceName, showOnStartup);
                    }
                }

                GUILayout.FlexibleSpace();

                if(discoverAsset.Debug)
                {
                    if (GUILayout.Button("Select DiscoverAsset"))
                        Selection.activeObject = discoverAsset;

                    if (GUILayout.Button("Reload"))
                        UpdateDiscoverObjects(true);
                }

                if (GUILayout.Button("Close"))
                {
                    Close();
                }
            }
        }

        Vector2 globalContentScroll;

        void GlobalContentGUI()
        {
            globalContentScroll = GUILayout.BeginScrollView(globalContentScroll);
            using (new GUILayout.VerticalScope(Styles.indent))
            {
                GUILayout.Label(discoverAsset.Title, Styles.header);
                using (new GUILayout.VerticalScope(Styles.indent))
                {
                    if(discoverAsset.Image != null)
                    {
                        DiscoverEditor.DrawImage(discoverAsset.Image);
                    }

                    GUILayout.Label(discoverAsset.Description, Styles.body);

                    if(discoverAsset.Scenes != null)
                    {
                        foreach (var map in discoverAsset.Scenes)
                        {
                            using (new GroupLabelScope(map.Title))
                            {
                                if(map.Image != null)
                                {
                                    DiscoverEditor.DrawImage(map.Image);
                                }

                                GUILayout.Label(map.Description, Styles.body);

                                using (new GUILayout.HorizontalScope())
                                {
                                    GUILayout.FlexibleSpace();

                                    if (map.SceneSetups != null)
                                    {
                                        foreach(var sceneSetup in map.SceneSetups)
                                        {
                                            if (sceneSetup != null && GUILayout.Button($"Open {sceneSetup.name}"))
                                                LoadSceneSetup(sceneSetup);
                                        }
                                    }
                                    if(map.SingleScenes != null)
                                    {
                                        foreach(var singleScene in map.SingleScenes)
                                        {
                                            if (singleScene != null && GUILayout.Button($"Open {singleScene.name}"))
                                                LoadSingleScene(singleScene);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
        }

        Discover selectedDiscover;
        Vector2 listScroll;
        Vector2 contentScroll;

        void SceneContentGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    listScroll = GUILayout.BeginScrollView(listScroll, GUI.skin.box, GUILayout.Width(discoverAsset.DiscoverListWidth));
                    using (new GUILayout.VerticalScope(GUILayout.ExpandHeight(true)))
                    {
                        foreach (var category in discoverObjects.Keys.OrderBy((x) => x.ToString()))
                        {
                            if(!string.IsNullOrEmpty(category))
                                GUILayout.Label(category, EditorStyles.boldLabel);

                            foreach (var item in discoverObjects[category])
                            {
                                EditorGUI.BeginChangeCheck();
                                bool value = GUILayout.Toggle(item == selectedDiscover, item.Name, Styles.listItem);

                                if (value)
                                {
                                    // Select the new one if not selected
                                    if(selectedDiscover != item)
                                    {
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            if (discoverAsset.Debug)
                                                Selection.activeObject = item;

                                            SetSelectedDiscover(item);
                                        }
                                    }

                                    Rect r = GUILayoutUtility.GetLastRect();
                                    int c = EditorGUIUtility.isProSkin ? 1 : 0;
                                    EditorGUI.DrawRect(r, new Color(c, c, c, 0.1f));
                                }
                            }
                        }

                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.Space(4);

                using (new GUILayout.VerticalScope(GUILayout.Width(440)))
                {
                    contentScroll = GUILayout.BeginScrollView(contentScroll);
                    GUILayout.Space(8);

                    DiscoverEditor.DrawDiscoverContentGUI(selectedDiscover);

                    GUILayout.FlexibleSpace();
                    GUILayout.EndScrollView();
                }
            }
        }

        void SetSelectedDiscover(Discover newSelection)
        {

            if (SceneView.lastActiveSceneView != null && newSelection.AlignViewToTransform)
            {
                SceneView.lastActiveSceneView.AlignViewToObject(newSelection.transform);
            }

            if (selectedDiscover.ObjectsToToggle != null)
            {
                // Reverse Toggle previous GameObjects state
                foreach (var go in selectedDiscover.ObjectsToToggle)
                {
                    if (go.GameObject == null)
                        continue;

                    switch (go.State)
                    {
                        case Actions.ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Disable:
                            go.GameObject.SetActive(true);
                            break;
                        case Actions.ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Enable:
                            go.GameObject.SetActive(false);
                            break;
                        case Actions.ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Toggle:
                            go.GameObject.SetActive(go.GameObject.activeSelf);
                            break;
                    }
                }
            }

            // Set the new item
            selectedDiscover = newSelection;

            if (selectedDiscover.ObjectsToToggle != null)
            {
                // Toggle Next GameObjects State
                foreach (var go in selectedDiscover.ObjectsToToggle)
                {
                    if (go.GameObject == null)
                        continue;

                    switch (go.State)
                    {
                        case Actions.ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Disable:
                            go.GameObject.SetActive(false);
                            break;
                        case Actions.ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Enable:
                            go.GameObject.SetActive(true);
                            break;
                        case Actions.ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Toggle:
                            go.GameObject.SetActive(go.GameObject.activeSelf);
                            break;
                    }
                }
            }
            contentScroll = Vector2.zero;
        }

        void LoadSceneSetup(EditorSceneSetup setup)
        {
            try
            {
                EditorUtility.DisplayProgressBar("Discover", $"Opening {setup.name}...", 0.9f);
                forceGlobal = false;
                EditorSceneSetup.RestoreSetup(setup);
            }
            catch
            {
                Debug.LogError($"Could not load EditorSceneSetup : {setup.name}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                UpdateDiscoverObjects();
            }
        }

        void LoadSingleScene(SceneAsset scene)
        {
            try
            {
                EditorUtility.DisplayProgressBar("Discover", $"Opening {scene.name}...", 0.9f);
                forceGlobal = false;
                EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene), OpenSceneMode.Single);
            }
            catch
            {
                Debug.LogError($"Could not load Scene : {scene.name}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                UpdateDiscoverObjects();
            }
        }

        public class GroupLabelScope : GUILayout.VerticalScope
        {
            public GroupLabelScope(string name) : base(Styles.box)
            {
                if(!string.IsNullOrWhiteSpace(name))
                {
                    GUIContent n = new GUIContent(name);
                    Rect r = GUILayoutUtility.GetRect(n, Styles.boxHeader, GUILayout.ExpandWidth(true));
                    GUI.Label(r, n, Styles.boxHeader);
                }

            }
        }

        public static class Styles
        {
            public static GUIStyle indent;
            public static GUIStyle slightIndent;

            public static GUIStyle header;
            public static GUIStyle subHeader;
            public static GUIStyle body;

            public static GUIStyle box;
            public static GUIStyle boxHeader;

            public static GUIStyle listItem;

            public static GUIStyle buttonLeft;
            public static GUIStyle buttonMid;
            public static GUIStyle buttonRight;

            public static GUIStyle tabContainer;

            public static GUIStyle image;

            static Styles()
            {
                header = new GUIStyle(EditorStyles.wordWrappedLabel);
                header.fontSize = 24;
                header.padding = new RectOffset(0, 0, -4, -4);
                header.richText = true;

                subHeader = new GUIStyle(EditorStyles.wordWrappedLabel);
                subHeader.fontSize = 11;
                subHeader.fontStyle = FontStyle.Italic;

                body = new GUIStyle(EditorStyles.wordWrappedLabel);
                body.fontSize = 11;
                body.richText = true;

                indent = new GUIStyle();
                indent.padding = new RectOffset(12, 12, 12, 12);

                slightIndent = new GUIStyle();
                slightIndent.padding = new RectOffset(6, 6, 0, 6);

                box = new GUIStyle(EditorStyles.helpBox);

                boxHeader = new GUIStyle(GUI.skin.box);
                boxHeader.normal.textColor = GUI.skin.label.normal.textColor;
                boxHeader.fixedHeight = 24;
                boxHeader.fontSize = 16;
                boxHeader.fontStyle = FontStyle.Bold;
                boxHeader.alignment = TextAnchor.UpperLeft;
                boxHeader.margin = new RectOffset(0, 0, 0, 6);

                listItem = new GUIStyle(EditorStyles.label);
                listItem.padding = new RectOffset(12, 0, 2, 2);

                buttonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
                buttonLeft.fontSize = 11;
                buttonMid = new GUIStyle(EditorStyles.miniButtonMid);
                buttonMid.fontSize = 11;
                buttonRight = new GUIStyle(EditorStyles.miniButtonRight);
                buttonRight.fontSize = 11;

                tabContainer = new GUIStyle(EditorStyles.miniButton);
                tabContainer.padding = new RectOffset(4, 4, 0, 0);

                image = new GUIStyle(GUIStyle.none);
                image.stretchWidth = true ;

            }
        }
    }

}
