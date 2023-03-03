using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine.SceneManagement;
using GameplayIngredients.Rigs;
using UnityEngine.Events;
using UnityEditor.SceneManagement;

namespace GameplayIngredients.Editor
{
    public class CheckWindow : EditorWindow
    {
        [MenuItem("Window/Gameplay Ingredients/Check and Resolve", priority = MenuItems.kWindowMenuPriority+30)]
        public static void OpenWindow()
        {
            GetWindow<CheckWindow>(false);
        }


        private void OnEnable()
        {
            titleContent = new GUIContent(EditorGUIUtility.IconContent("Valid"));
            titleContent.text = "Check/Resolve";
            minSize = new Vector2(640, 180);
            InitializeCheckStates();
        }

        void InitializeCheckStates()
        {
            s_CheckStates = new Dictionary<Check, bool>();
            foreach (var check in Check.allChecks)
            {
                s_CheckStates.Add(check, EditorPrefs.GetBool(kPreferencePrefix + check.name, check.defaultEnabled));
            }
        }

        void SetCheckState(Check check, bool value)
        {
            s_CheckStates[check] = value;
            EditorPrefs.SetBool(kPreferencePrefix + check.name, value);
        }

        const string kPreferencePrefix = "GameplayIngredients.Check.";
        const string kShowIgnoredPreference = "GameplayIngredients.ShowIgnored";

        bool showIgnored
        {
            get { 
                return EditorPrefs.GetBool(kShowIgnoredPreference, true); 
            }
            set { 
                EditorPrefs.SetBool(kShowIgnoredPreference, value); 
            }
        }

        Vector2 Scroll = new Vector2();
        Dictionary<Check, bool> s_CheckStates;

        int noticeCount;
        int warningCount;
        int errorCount;

        enum SortMode
        {
            None,
            CheckType,
            ObjectName,
            Message,
            Resolution,
            Ignored
        }

        SortMode sortMode = SortMode.None;
        bool invertSort = false;

        string filterString = "";

        bool showNotice
        {
            get
            {
                return EditorPrefs.GetBool(kPreference + "showNotice", true);
            }
            set
            {
                if (value != showNotice)
                    EditorPrefs.SetBool(kPreference + "showNotice", value);
            }
        }
        bool showWarning
        {
            get
            {
                return EditorPrefs.GetBool(kPreference + "showWarning", true);
            }
            set
            {
                if (value != showWarning)
                    EditorPrefs.SetBool(kPreference + "showWarning", value);
            }
        }

        bool showError
        {
            get
            {
                return EditorPrefs.GetBool(kPreference + "showError", true);
            }
            set
            {
                if (value != showError)
                    EditorPrefs.SetBool(kPreference + "showError", value);
            }
        }

        string kPreference = "GameplayIngredients.CheckResolve.";

        private void SortButton(string label, SortMode sortMode, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(label, this.sortMode == sortMode ? Styles.sortHeader : Styles.header, options))
            {

                if (this.sortMode == sortMode)
                    invertSort = !invertSort;
                else
                {
                    this.sortMode = sortMode;
                    invertSort = false;
                }

                if(m_Results != null)
                {
                    switch (sortMode)
                    {
                        default:
                        case SortMode.None:
                            break;
                        case SortMode.CheckType:
                            m_Results = m_Results.OrderBy((a) => a.check.name).ToList();
                            break;
                        case SortMode.ObjectName:
                            m_Results = m_Results.OrderBy((a) => a.mainObject == null? "" : a.mainObject.name).ToList();
                            break;
                        case SortMode.Message:
                            m_Results = m_Results.OrderBy((a) => a.message.text).OrderBy((a) => a.result).ToList();
                            break;
                        case SortMode.Resolution:
                            m_Results = m_Results.OrderBy((a) => a.check.ResolutionActions[a.resolutionActionIndex]).ToList();
                            break;
                        case SortMode.Ignored:
                            m_Results = m_Results.OrderBy((a) => IsIgnored(a)).ToList();
                            break;
                    }
                    if(invertSort)
                    {
                        m_Results.Reverse();
                    }

                    Repaint();
                }
            }
        }

        private void OnGUI()
        {
            using(new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Height(22)))
            {
                if (GUILayout.Button("Check", EditorStyles.toolbarButton))
                    PerformChecks();
                if(GUILayout.Button("", EditorStyles.toolbarPopup))
                {
                    Rect r = new Rect(0, 0, 16, 20);
                    GenericMenu menu = new GenericMenu();
                    foreach (Check check in s_CheckStates.Keys)
                    {
                        menu.AddItem(new GUIContent(check.name), s_CheckStates[check], () => SetCheckState(check, !s_CheckStates[check]));
                    }
                    menu.DropDown(r);
                }

                if (GUILayout.Button("Resolve", EditorStyles.toolbarButton))
                    Resolve();

                GUILayout.FlexibleSpace();

                filterString = EditorGUILayout.DelayedTextField(filterString, EditorStyles.toolbarSearchField, GUILayout.Width(180));

                showIgnored = GUILayout.Toggle(showIgnored, "Ignored", EditorStyles.toolbarButton);
                showNotice = GUILayout.Toggle(showNotice, new GUIContent(noticeCount.ToString(), CheckResult.GetIcon(CheckResult.Result.Notice)), EditorStyles.toolbarButton);
                showWarning = GUILayout.Toggle(showWarning, new GUIContent(warningCount.ToString(), CheckResult.GetIcon(CheckResult.Result.Warning)), EditorStyles.toolbarButton);
                showError = GUILayout.Toggle(showError, new GUIContent(errorCount.ToString(), CheckResult.GetIcon(CheckResult.Result.Failed)), EditorStyles.toolbarButton);

            }
            using(new GUILayout.VerticalScope())
            {
                GUI.backgroundColor = Color.white * 1.3f;
                using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    SortButton("Check Type", SortMode.CheckType, GUILayout.Width(128));
                    SortButton("Object", SortMode.ObjectName, GUILayout.Width(128));
                    SortButton("Message", SortMode.Message, GUILayout.ExpandWidth(true));
                    SortButton("Resolution", SortMode.Resolution, GUILayout.Width(128));
                    SortButton("Ignored", SortMode.Ignored, GUILayout.Width(64));
                    GUILayout.Space(12);
                }

                Scroll = GUILayout.BeginScrollView(Scroll, false,true);

                if (m_Results != null && m_Results.Count > 0)
                {
                    bool odd = true;
                    foreach (var result in m_Results)
                    {
                        if (!showIgnored && IsIgnored(result) || result.mainObject == null)
                            continue;

                        if (result.result == CheckResult.Result.Notice && !showNotice)
                            continue;

                        if (result.result == CheckResult.Result.Warning && !showWarning)
                            continue;

                        if (result.result == CheckResult.Result.Failed && !showError)
                            continue;

                        if (!string.IsNullOrEmpty(filterString) && !result.message.text.Contains(filterString))
                            continue;

                        GUI.backgroundColor = Color.white * (odd ? 0.9f : 0.8f);
                        odd = !odd;

                        using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                        {
                            EditorGUI.BeginDisabledGroup(showIgnored && IsIgnored(result));
                            
                            GUILayout.Label(result.check.name, Styles.line, GUILayout.Width(128));
                            if(GUILayout.Button(result.mainObject != null? result.mainObject.name : "Null", Styles.line, GUILayout.Width(128)))
                            {
                                Selection.activeObject = result.mainObject;
                            }
                            GUILayout.Label(result.message, Styles.line, GUILayout.ExpandWidth(true));
                            ShowMenu(result);

                            EditorGUI.EndDisabledGroup();

                            GUILayout.Space(18);
                            EditorGUI.BeginChangeCheck();
                            var ignored = GUILayout.Toggle(IsIgnored(result),"", EditorStyles.toggle ,GUILayout.Width(24));
                            if (EditorGUI.EndChangeCheck())
                            {
                                SetIgnored(result, ignored);
                            }
                            GUILayout.Space(18);

                        }
                    }
                }
                else
                {
                    GUILayout.Label("No Results");
                }
                GUI.backgroundColor = Color.white;

                GUILayout.FlexibleSpace();
                GUILayout.EndScrollView();
            }
        }

        Dictionary<Scene,IgnoredCheckResults> m_IgnoredLists;
        
        void BuildIgnoredList()
        {
            if (m_IgnoredLists == null)
                m_IgnoredLists = new Dictionary<Scene, IgnoredCheckResults>();
            else
                m_IgnoredLists.Clear();

            var all = FindObjectsOfType<IgnoredCheckResults>().ToList();
            foreach(var one in all)
            {
                if (!m_IgnoredLists.ContainsKey(one.gameObject.scene))
                    m_IgnoredLists.Add(one.gameObject.scene, one);
                else
                    Debug.LogWarning($"Found at least two IgnoredCheckResults objects in scene {one.gameObject.scene.name}");
            }
        }

        bool IsIgnored(CheckResult result)
        {
            if (result.mainObject == null || !(result.mainObject is GameObject))
                return false;

            GameObject go = result.mainObject as GameObject;
            if (!m_IgnoredLists.ContainsKey(go.scene))
                return false;

            var igl = m_IgnoredLists[go.scene];
            return igl.ignoredCheckResults.Any(o => (o.check == result.check.GetType().ToString()) && (o.gameObject == go));
        }

        void SetIgnored(CheckResult result, bool ignored)
        {
            result.SetIgnored(ignored);
            if(result.mainObject is GameObject)
            {
                var go = result.mainObject as GameObject;
                EditorSceneManager.MarkSceneDirty(go.scene);
            }
            BuildIgnoredList();
        }

        void ShowMenu(CheckResult result)
        {
            if (s_IntValues == null)
                s_IntValues = new Dictionary<Check, int[]>();

            if(!s_IntValues.ContainsKey(result.check))
            {
                int count = result.check.ResolutionActions.Length;
                int[] values = new int[count];
                for(int i = 0; i < count; i++)
                {
                    values[i] = i;
                }
                s_IntValues.Add(result.check, values);
            }

            result.resolutionActionIndex = EditorGUILayout.IntPopup(result.resolutionActionIndex, result.check.ResolutionActions, s_IntValues[result.check], EditorStyles.toolbarDropDown, GUILayout.Width(128));

        }

        static Dictionary<Check, int[]> s_IntValues;

        List<CheckResult> m_Results = new List<CheckResult>();

        void Resolve()
        {
            foreach(var result in m_Results)
            {
                if(!IsIgnored(result))
                    result.check.Resolve(result);
            }
            PerformChecks();
        }

        void PerformChecks()
        {
            List<CheckResult> results = new List<CheckResult>();
            bool canceled = false;
            try
            {
                var so = new SceneObjects();

                int count = s_CheckStates.Count;
                int i = 0;
                foreach (var kvp in s_CheckStates)
                {
                    float t = (float)i / count;
                    if (EditorUtility.DisplayCancelableProgressBar("Performing Checks", kvp.Key.name, t))
                    {
                        canceled = true;
                        break;
                    }

                    if (kvp.Value)
                        results.AddRange(kvp.Key.GetResults(so));
                    i++;
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if(!canceled)
                m_Results = results;

            sortMode = SortMode.None;

            noticeCount = m_Results.Where(o => o.result == CheckResult.Result.Notice).Count();
            warningCount = m_Results.Where(o => o.result == CheckResult.Result.Warning).Count();
            errorCount = m_Results.Where(o => o.result == CheckResult.Result.Failed).Count();

            BuildIgnoredList();
            Repaint();
        }


        static class Styles
        {
            public static GUIStyle header;
            public static GUIStyle sortHeader;
            public static GUIStyle line;

            static Styles()
            {
                header = new GUIStyle(EditorStyles.toolbarButton);
                header.alignment = TextAnchor.MiddleLeft;
                header.fontStyle = FontStyle.Bold;

                sortHeader = new GUIStyle(EditorStyles.toolbarDropDown);
                sortHeader.alignment = TextAnchor.MiddleLeft;
                sortHeader.fontStyle = FontStyle.Bold;

                line = new GUIStyle(EditorStyles.toolbarButton);
                line.alignment = TextAnchor.MiddleLeft;

            }
        }
    }

    public class SceneObjects
    {
        public GameObject[] allObjects;
        public List<GameObject> referencedGameObjects;
        public List<Component> referencedComponents;
        public List<UnityEngine.Object> referencedObjects;

        public SceneObjects()
        {
            allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            allObjects = allObjects.Where(o => !PrefabUtility.IsPartOfPrefabAsset(o) && o.hideFlags == HideFlags.None).ToArray();

            referencedGameObjects = new List<GameObject>();
            referencedComponents = new List<Component>();
            referencedObjects = new List<UnityEngine.Object>();

            if (allObjects == null || allObjects.Length == 0)
                return;

            try
            {
                int count = allObjects.Length;
                int i = 0;

                foreach (var sceneObject in allObjects)
                {

                    float progress = ++i / (float)count;
                    if (EditorUtility.DisplayCancelableProgressBar("Building Reference Table...", $"{sceneObject.name}", progress))
                    {
                        referencedComponents.Clear();
                        referencedGameObjects.Clear();
                        referencedObjects.Clear();
                        break;
                    }

                    var components = sceneObject.GetComponents<Component>();
                    foreach (var component in components)
                    {
                        if (component is Transform)
                            continue;
                        else if(component is Renderer)
                        {
                            var renderer = component as Renderer;
                            if (renderer.probeAnchor != null)
                            {
                                referencedComponents.Add(renderer.probeAnchor);
                                referencedGameObjects.Add(renderer.probeAnchor.gameObject);
                            }
                            foreach(var sharedMat in renderer.sharedMaterials)
                            {
                                referencedObjects.Add(sharedMat);
                            }
                        }
                        else 
                        {
                            Type t = component.GetType();
                            FieldInfo[] fields = t.GetFields(BindingFlags.Public
                                             | BindingFlags.Instance
                                             | BindingFlags.NonPublic);

                            foreach (var f in fields)
                            {
                                if (f.FieldType == typeof(GameObject))
                                {
                                    var go = f.GetValue(component) as GameObject;
                                    if (go != null && go != component.gameObject)
                                    {
                                        referencedGameObjects.Add(go);
                                    }
                                }
                                else if (f.FieldType == typeof(GameObject[]))
                                {
                                    var golist = f.GetValue(component) as GameObject[];
                                    foreach (var go in golist)
                                        if (go != null && go != component.gameObject)
                                        {
                                            referencedGameObjects.Add(go);
                                        }
                                }
                                else if (f.FieldType == typeof(Transform))
                                {
                                    var tr = f.GetValue(component) as Transform;
                                    if (tr != null && tr.gameObject != component.gameObject)
                                    {
                                        referencedGameObjects.Add(tr.gameObject);
                                        referencedComponents.Add(tr);
                                    }
                                }
                                else if (f.FieldType == typeof(Transform[]))
                                {
                                    var trlist = f.GetValue(component) as Transform[];
                                    foreach (var tr in trlist)
                                        if (tr != null && tr.gameObject != component.gameObject)
                                        {
                                            referencedGameObjects.Add(tr.gameObject);
                                            referencedComponents.Add(tr);
                                        }
                                }
                                else if (f.FieldType.IsSubclassOf(typeof(Component)))
                                {
                                    var comp = f.GetValue(component) as Component;
                                    if (comp != null && comp.gameObject != component.gameObject)
                                    {
                                        referencedGameObjects.Add(comp.gameObject);
                                        referencedComponents.Add(comp);
                                    }
                                }
                                else if (f.FieldType.IsSubclassOf(typeof(Component[])))
                                {
                                    var complist = f.GetValue(component) as Component[];
                                    foreach(var comp in complist)
                                        if (comp != null && comp.gameObject != component.gameObject)
                                        {
                                            referencedGameObjects.Add(comp.gameObject);
                                            referencedComponents.Add(comp);
                                        }
                                }
                                else if (f.FieldType == typeof(UnityEvent))
                                {
                                    var ue = f.GetValue(component) as UnityEvent;
                                    int evtCount = ue.GetPersistentEventCount();
                                    for(int k = 0; k< evtCount; k++)
                                    {
                                        var target = ue.GetPersistentTarget(k);
                                        if (target is GameObject)
                                            referencedGameObjects.Add(target as GameObject);
                                        else if(target.GetType().IsSubclassOf(typeof(Component)))
                                            referencedGameObjects.Add((target as Component).gameObject);
                                    }
                                }
                                else if (f.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                                {
                                    var obj = f.GetValue(component) as UnityEngine.Object;
                                    referencedObjects.Add(obj);
                                }

                            }
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }
    }

}
