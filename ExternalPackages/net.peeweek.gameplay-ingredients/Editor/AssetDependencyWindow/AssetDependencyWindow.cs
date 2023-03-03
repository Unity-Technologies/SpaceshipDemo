using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using System.Reflection;

public class AssetDependencyWindow : EditorWindow
{
    public static AssetDependencyWindow instance { get => s_Instance; }
    static AssetDependencyWindow s_Instance;

    static bool includeMonoScript = false;
    static bool includeSubAssets = false;

    [MenuItem("Assets/Show Asset Dependencies... #F4", validate = true)]
    static bool CanOpen()
    {
        return Selection.activeObject != null;
    }

    [MenuItem("Assets/Show Asset Dependencies... #F4")]
    static void Open()
    {
        if (s_Instance == null)
            s_Instance = GetWindow<AssetDependencyWindow>();
        else
            s_Instance.Focus();

        if(Selection.objects.Length > 0)
        {
            foreach(var obj  in Selection.objects)
            {
                s_Instance.WatchAsset(obj);
            }

            if (s_Instance.dtv == null)
                s_Instance.dtv = new DependencyTreeView(new TreeViewState());

            s_Instance.dtv.Reload();
        }
    }

    static List<Object> watched;

    void WatchAsset(Object obj)
    {
        if (obj == null)
            return;

        if (watched == null)
            watched = new List<Object>();

        if (!watched.Contains(obj))
            watched.Add(obj);
    }

    private void OnEnable()
    {
        var title = EditorGUIUtility.IconContent("Project");
        title.text = "Asset Dependencies";
        titleContent = new GUIContent(title);
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    DependencyTreeView dtv;

    private void OnGUI()
    {
        using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            EditorGUI.BeginChangeCheck();
            includeMonoScript = GUILayout.Toggle(includeMonoScript, "MonoScript", EditorStyles.toolbarButton);
            includeSubAssets = GUILayout.Toggle(includeSubAssets, "SubAssets", EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
                dtv.Reload();
            
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                watched.Clear();
                dtv.Reload();
            }
        }

        if (dtv == null)
        {
            dtv = new DependencyTreeView(new TreeViewState());
            dtv.Reload();
        }

        GUILayout.FlexibleSpace();
        dtv.OnGUI(new Rect(0,22,position.width, position.height -22));
    }

    class DependencyTreeView : TreeView
    {
        Dictionary<int, DependencyTreeViewItem> items;

        public DependencyTreeView(TreeViewState tvs): base(tvs)
        {
            items = null;
        }

        protected override TreeViewItem BuildRoot()
        {
            if (items == null)
                items = new Dictionary<int, DependencyTreeViewItem>(); 

            items.Clear();

            int id = -1;
            var tvi = new TreeViewItem(++id, -1, "Root");

            if(watched == null || watched.Count == 0)
                tvi.AddChild(AddWatchedItem(null, 0, ref id));
            else
            {
                try
                {
                    int i = 0;
                    foreach (var item in watched)
                    {
                        EditorUtility.DisplayProgressBar("Asset Dependencies", $"Loading info for {item.name}...", (float)i/watched.Count);
                        tvi.AddChild(AddWatchedItem(item, 0, ref id));
                        i++;
                    }

                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }


            return tvi;
        }

        DependencyTreeViewItem AddWatchedItem(Object watchedObject, int depth, ref int id)
        {
            int i = ++id;
            var tvi = new DependencyTreeViewItem(i, depth, watchedObject);
            items.Add(i, tvi);

            List<Object> dependencies = new List<Object>();

            if (includeSubAssets && depth == 0)
            {
                // Search all SubAssets (only if this is the main asset)
                foreach (var c in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(watchedObject)))
                {
                    if (c == watchedObject)
                        continue;
                    else if(c.hideFlags != 0)
                        dependencies.Add(c);
                }
            }

            // If Game object
            if (watchedObject is GameObject)
            {
                foreach (var component in (watchedObject as GameObject).GetComponents<Component>())
                {
                    dependencies.Add(component);
                }
            }


            // Search for dependencies
            foreach (var d in AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(watchedObject), false))
            {
                dependencies.Add(AssetDatabase.LoadMainAssetAtPath(d));
            }


            foreach (var depObj in dependencies)
            {

                if (depObj == watchedObject)
                {
                    Debug.LogWarning("Duplicate");
                    continue;
                }

                if (depObj.GetType() == typeof(MonoScript) && !includeMonoScript)
                    continue;

                var tvc = AddWatchedItem(depObj, depth + 1, ref id);
                tvi.AddChild(tvc);

            }

            return tvi;
        }

        protected override void DoubleClickedItem(int id)
        {
            var obj = items[id].target;
            if (obj != null)
                Selection.activeObject = obj;
        }

        class DependencyTreeViewItem : TreeViewItem
        {
            public readonly Object target;

            public DependencyTreeViewItem(int id, int depth, Object obj) : base(id, depth, obj == null? "(No Items)" : obj.name)
            {
                target = obj;
                try
                {
                    this.icon = EditorGUIUtility.ObjectContent(obj, obj.GetType()).image as Texture2D;
                    this.displayName = $"{obj.name} ({obj.GetType().Name})";
                }
                catch
                {
                    this.icon = EditorGUIUtility.IconContent("GameObject Icon").image as Texture2D;
                }

            }
        }
    }

    static class Styles
    {
        public static GUIStyle toolbarButton;
        public static Color highlightColor = new Color(1.0f, 1.5f, 2.0f);

        static Styles()
        {
            toolbarButton = new GUIStyle(EditorStyles.toolbarButton);
            toolbarButton.alignment = TextAnchor.MiddleLeft;
        }

    }
}
