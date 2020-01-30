using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace GameplayIngredients.Editor
{
    public class FindAndReplaceWindow : EditorWindow
    {
        [MenuItem("Edit/Find And Replace... %&#F", priority = 144)]
        static void OpenWindow()
        {
            GetWindow<FindAndReplaceWindow>();
        }

        static readonly Dictionary<string, Type> s_assemblyTypes = GetTypes();

        private static Dictionary<string, Type> GetTypes()
        {
            Dictionary<string, Type> all = new Dictionary<string, Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (typeof(Component).IsAssignableFrom(t) && !all.ContainsKey(t.Name))
                        all.Add(t.Name, t);
                }
            }

            return all;
        }



        public enum SearchBy
        {
            Name,
            ComponentType,
            Tag,
            Layer,
            Mesh,
            Material,
            Selection
        }

        private void OnEnable()
        {
            titleContent = Contents.title;
            minSize = new Vector2(640, 280);
        }

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope(GUILayout.Width(320)))
                {
                    SearchControlsGUI();
                }

                using (new GUILayout.VerticalScope(GUILayout.Width(4)))
                {
                    GUILayout.FlexibleSpace();
                    Rect r = GUILayoutUtility.GetLastRect();
                    EditorGUI.DrawRect(r, Color.black);
                }

                using (new GUILayout.VerticalScope())
                {
                    SearchResultsGUI();
                }

            }
        }

        [SerializeField]
        GameObject prefabReplacement;
        [SerializeField]
        SearchBy searchBy;

        [SerializeField]
        string nameSearch = "GameObject";
        [SerializeField]
        string tagSearch = "Player";
        [SerializeField]
        string layerSearch = "PostProcessing";
        [SerializeField]
        string componentSearch = "Light";
        [SerializeField]
        Mesh meshSearch;
        [SerializeField]
        Material materialSearch;
        [SerializeField]
        bool selectionRecurse = false;

        [SerializeField]
        bool keepPosition = true;
        [SerializeField]
        bool keepRotation = true;
        [SerializeField]
        bool keepScale = false;
        [SerializeField]
        bool keepParenting = true;
        [SerializeField]
        bool keepName = true;
        [SerializeField]
        bool keepTag = false;
        [SerializeField]
        bool keepLayer = true;
        [SerializeField]
        bool keepStatic = false;
        [SerializeField]
        bool unpackPrefab = false;

        enum SearchOp
        {
            Find,
            Add,
            Refine
        }

        void SearchControlsGUI()
        {
            EditorGUIUtility.labelWidth = 120;
            GUILayout.Space(4);
            GUILayout.Label("Search Scene Objects", Styles.boldLabel);
            searchBy = (SearchBy)EditorGUILayout.EnumPopup(Contents.criteria, searchBy);
            switch (searchBy)
            {
                case SearchBy.Name:
                    nameSearch = EditorGUILayout.TextField(Contents.nameSearch, nameSearch);
                    SearchButtonsGUI(searchBy, nameSearch);
                    break;
                case SearchBy.Tag:
                    tagSearch = EditorGUILayout.TextField(Contents.tagSearch, tagSearch);
                    SearchButtonsGUI(searchBy, tagSearch);
                    break;
                case SearchBy.Layer:
                    layerSearch = EditorGUILayout.TextField(Contents.layerSearch, layerSearch);
                    SearchButtonsGUI(searchBy, layerSearch);
                    break;
                case SearchBy.ComponentType:
                    componentSearch = EditorGUILayout.TextField(Contents.componentSearch, componentSearch);
                    SearchButtonsGUI(searchBy, componentSearch);
                    break;
                case SearchBy.Mesh:
                    meshSearch = (Mesh)EditorGUILayout.ObjectField(Contents.meshSearch, meshSearch, typeof(Mesh), true);
                    SearchButtonsGUI(searchBy, meshSearch);
                    break;
                case SearchBy.Material:
                    materialSearch = (Material)EditorGUILayout.ObjectField(Contents.materialSearch, materialSearch, typeof(Material), true);
                    SearchButtonsGUI(searchBy, materialSearch);
                    break;
                case SearchBy.Selection:
                    selectionRecurse = EditorGUILayout.Toggle(Contents.selectionRecurse, selectionRecurse);
                    SearchButtonsGUI(searchBy, selectionRecurse);
                    break;

            }


            GUILayout.FlexibleSpace();
            GUILayout.Label("Replace Results", Styles.boldLabel);
            prefabReplacement = (GameObject)EditorGUILayout.ObjectField(Contents.prefabReplacement, prefabReplacement, typeof(GameObject), true);

            if (prefabReplacement != null)
            {
                PrefabAssetType type = PrefabUtility.GetPrefabAssetType(prefabReplacement);
                bool isAPrefab = type == PrefabAssetType.Model || type == PrefabAssetType.Regular || type == PrefabAssetType.Variant;
                if(isAPrefab)
                    unpackPrefab = EditorGUILayout.Toggle("Unpack Prefab", unpackPrefab);
            }
            EditorGUI.BeginDisabledGroup(prefabReplacement == null);
             
            GUILayout.Label("Keep Properties from Original:");

            using (new GUILayout.HorizontalScope())
            {
                keepPosition = GUILayout.Toggle(keepPosition, "Position", EditorStyles.miniButtonLeft, GUILayout.Height(16));
                keepRotation = GUILayout.Toggle(keepRotation, "Rotation", EditorStyles.miniButtonMid, GUILayout.Height(16));
                keepScale = GUILayout.Toggle(keepScale, "Scale", EditorStyles.miniButtonMid, GUILayout.Height(16));
                keepParenting = GUILayout.Toggle(keepParenting, "Parenting", EditorStyles.miniButtonRight, GUILayout.Height(16));
            }
            using (new GUILayout.HorizontalScope())
            {
                keepName = GUILayout.Toggle(keepName, "Name", EditorStyles.miniButtonLeft, GUILayout.Height(16));
                keepTag = GUILayout.Toggle(keepTag, "Tag", EditorStyles.miniButtonMid, GUILayout.Height(16));
                keepLayer = GUILayout.Toggle(keepLayer, "Layer", EditorStyles.miniButtonMid, GUILayout.Height(16));
                keepStatic = GUILayout.Toggle(keepStatic, "Static", EditorStyles.miniButtonRight, GUILayout.Height(16));
            }

            if (GUILayout.Button("Replace All", Styles.bigButton, GUILayout.Height(24)) && prefabReplacement != null)
            {
                Undo.RecordObjects(searchResults.ToArray(), "Replace Objects");

                for(int i = 0; i < searchResults.Count; i++)
                {
                    var obj = searchResults[i];
                    var newObj = SwapObject(obj, prefabReplacement, searchResults);
                    searchResults[i] = newObj;
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(8);
        }

        GameObject SwapObject(GameObject toReplace, GameObject replacement, List<GameObject> others)
        {
            GameObject newObj;

            if (PrefabUtility.GetPrefabAssetType(replacement) != PrefabAssetType.NotAPrefab && !unpackPrefab)
                newObj = (GameObject)PrefabUtility.InstantiatePrefab(replacement);
            else
                newObj = Instantiate(replacement);

            if (keepName)
                newObj.name = toReplace.name;

            if(keepPosition)
                newObj.transform.position = toReplace.transform.position;
            if(keepRotation)
                newObj.transform.rotation = toReplace.transform.rotation;
            if(keepParenting)
                newObj.transform.parent = toReplace.transform.parent;
            if(keepScale)
                newObj.transform.localScale = toReplace.transform.localScale;

            if(keepTag)
                newObj.tag = toReplace.tag;
            if(keepLayer)
                newObj.layer = toReplace.layer;
            if(keepStatic)
                newObj.isStatic = toReplace.isStatic;

            foreach(var other in others)
            {
                if(other.transform.parent == toReplace.transform)
                {
                    other.transform.parent = newObj.transform;
                }
            }

            DestroyImmediate(toReplace);

            return newObj;
        }

        void SearchButtonsGUI(SearchBy by, object criteria)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Find", Styles.bigButton, GUILayout.Height(24)))
                    Search(SearchOp.Find, by, criteria);

                if (GUILayout.Button("Add", Styles.bigButton, GUILayout.Height(24)))
                    Search(SearchOp.Add, by, criteria);

                if (GUILayout.Button("Refine", Styles.bigButton, GUILayout.Height(24)))
                    Search(SearchOp.Refine, by, criteria);

            }
        }

        static GameObject[] FindAllSceneObjects()
        {
            var all = Resources.FindObjectsOfTypeAll<GameObject>();
            all = all.Where(o => o.scene.isLoaded).ToArray();
            return all;
        }

        void Search(SearchOp op, SearchBy by, object criteria)
        {
            List<GameObject> query = new List<GameObject>();

            var all = FindAllSceneObjects();

            switch (by)
            {
                case SearchBy.Name:
                    foreach(var go in all)
                    {
                        if (go.name.Contains((string)criteria))
                            query.Add(go);
                    }
                    break;
                case SearchBy.Tag:
                    query.AddRange(GameObject.FindGameObjectsWithTag((string)criteria));
                    break;
                case SearchBy.Layer:
                    foreach (var go in all)
                    {
                        if (go.layer == LayerMask.NameToLayer((string)criteria))
                            query.Add(go);
                    }
                    break;
                case SearchBy.ComponentType:
                    if(s_assemblyTypes.ContainsKey((string)criteria))
                    {
                        Type t = s_assemblyTypes[(string)criteria];
                        if( typeof(Component).IsAssignableFrom(t))
                        {
                            Component[] components = (Component[])Resources.FindObjectsOfTypeAll(t);
                            if(components != null)
                            {
                                foreach(var c in components)
                                {
                                    if (c.gameObject.scene != null && !query.Contains(c.gameObject))
                                        query.Add(c.gameObject);
                                }
                            }
                        }
                    }
                    break;
                case SearchBy.Mesh:
                    Mesh mesh = (Mesh)criteria;
                    foreach (var go in all)
                    {
                        MeshFilter filter = go.GetComponent<MeshFilter>();
                        if (filter != null && filter.sharedMesh == mesh)
                        {
                            query.Add(go);
                        }
                    }
                    break;
                case SearchBy.Material:
                    Material mat = (Material)criteria;
                    foreach (var go in all)
                    {
                        Renderer renderer = go.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            if(renderer.sharedMaterials.Contains(mat))
                            {
                                query.Add(go);
                            }
                        }
                    }
                    break;
                case SearchBy.Selection:

                    foreach(var selected in Selection.gameObjects)
                    {
                        bool recurse = (bool)criteria;
                        if(!recurse)
                            query.Add(selected);
                        else
                            query.AddRange(selected.GetAllChildren());
                    }
                    break;
            }

            switch (op)
            {
                case SearchOp.Find:
                    searchResults = query;
                    break;
                case SearchOp.Add:
                    foreach(var item in query)
                    {
                        if (!searchResults.Contains(item))
                            searchResults.Add(item);
                    }
                    break;
                case SearchOp.Refine:
                    List<GameObject> refined = new List<GameObject>();
                    foreach (var item in searchResults)
                    {
                        if (query.Contains(item))
                            refined.Add(item);
                    }
                    searchResults = refined;
                    break;
            }
        }

        [SerializeField]
        List<GameObject> searchResults= new List<GameObject>();
        Vector2 scroll;

        void SearchResultsGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Search Results", Styles.boldLabel);
                GUILayout.FlexibleSpace();

                if(GUILayout.Button("Select in Scene", GUILayout.Height(24)))
                {
                    Selection.objects = searchResults.ToArray();
                }
                if(GUILayout.Button("Clear", GUILayout.Height(24)))
                {
                    searchResults.Clear();
                }
            }

            scroll = GUILayout.BeginScrollView(scroll, EditorStyles.helpBox);
            {
                GameObject toRemove = null;

                // Trim all nulls
                searchResults = searchResults.Where(o => o != null).ToList();

                foreach(var obj in searchResults)
                {
                    using (new GUILayout.HorizontalScope(EditorStyles.textField))
                    {
                        GUILayout.Label(obj.name, EditorStyles.label);
                        if(GUILayout.Button("X", GUILayout.Width(32)))
                        {
                            toRemove = obj;
                        }
                    }
                }

                if (toRemove != null)
                    searchResults.Remove(toRemove);
            }
            GUILayout.EndScrollView();

        }

        static class Contents
        {
            public static GUIContent title = new GUIContent("Find and Replace", (Texture)EditorGUIUtility.LoadRequired("ViewToolZoom On"));
            public static GUIContent criteria = new GUIContent("Criteria");
            public static GUIContent nameSearch = new GUIContent("GameObject Name");
            public static GUIContent tagSearch = new GUIContent("Tag");
            public static GUIContent layerSearch = new GUIContent("Layer");
            public static GUIContent componentSearch = new GUIContent("Component Type");
            public static GUIContent meshSearch = new GUIContent("Mesh");
            public static GUIContent materialSearch = new GUIContent("Material");
            public static GUIContent selectionRecurse = new GUIContent("Include Children");
            public static GUIContent prefabReplacement = new GUIContent("Prefab Replacement");
        }

        static class Styles
        {
            public static readonly GUIStyle boldLabel = GetBoldLabel();
            public static readonly GUIStyle bigButton = GetBigButton();

            static GUIStyle GetBoldLabel()
            {
                var style = new GUIStyle(EditorStyles.boldLabel);
                style.fontSize = 14;
                return style;
            }

            static GUIStyle GetBigButton()
            {
                var style = new GUIStyle(EditorStyles.miniButton);
                style.fontSize = 14;
                return style;
            }
        }
    }
}
