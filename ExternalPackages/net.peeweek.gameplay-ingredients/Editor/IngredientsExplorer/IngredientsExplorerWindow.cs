using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using GameplayIngredients.Events;
using GameplayIngredients.Logic;
using GameplayIngredients.Actions;
using GameplayIngredients.StateMachines;
using UnityEngine.SceneManagement;
using GameplayIngredients.Rigs;

namespace GameplayIngredients.Editor
{
    public class IngredientsExplorerWindow : EditorWindow
    {
        CallTreeView m_TreeView;
        [MenuItem("Window/Gameplay Ingredients/Ingredients Explorer", priority = MenuItems.kWindowMenuPriority + 30)]
        internal static void OpenWindow()
        {
            s_Instance = GetWindow<IngredientsExplorerWindow>();
        }

        internal static void OpenWindow(MonoBehaviour selected)
        {
            OpenWindow();
            instance.Repaint();
            instance.SelectItem(selected);
        }

        const int PANEL_WIDTH = 400;
        const int MIN_WIDTH = PANEL_WIDTH+60;

        const string kPref = "GameplayIngredients.IngredientsExplorer.ShowPanel";
        static bool showPanel
        {
            get => EditorPrefs.GetBool(kPref, false);
            set => EditorPrefs.SetBool(kPref, value);
        }

        public static bool visible = false;
        public static IngredientsExplorerWindow instance 
        { 
            get 
            { 
                if (s_Instance == null) 
                    s_Instance = GetWindow<IngredientsExplorerWindow>(); 
                return s_Instance; 
            } 
        }

        [SerializeField]
        static IngredientsExplorerWindow s_Instance;

        private void OnDisable()
        {
            visible = false;
            s_Instance = null;
            EditorSceneManager.sceneOpened -= Reload;
            EditorSceneSetup.onSetupLoaded -= ReloadSetup;
            Selection.selectionChanged -= Refresh;
        }

        private void OnEnable()
        {
            nodeRoots = new Dictionary<string, List<CallTreeNode>>();
            m_TreeView = new CallTreeView(nodeRoots);
            titleContent = new GUIContent("Ingredients Explorer", CallTreeView.Styles.Callable);
            ReloadCallHierarchy();
            EditorSceneManager.sceneOpened += Reload;
            EditorSceneSetup.onSetupLoaded += ReloadSetup;
            Selection.selectionChanged += Refresh;

            visible = true;
        }

        void Reload(Scene scene, OpenSceneMode mode)
        {
            ReloadCallHierarchy();
        }

        void ReloadSetup(EditorSceneSetup setup)
        {
            ReloadCallHierarchy();
        }

        public static void Refresh()
        {
            instance.ReloadCallHierarchy();
            instance.Repaint();
        }

        private void OnGUI()
        {
            int tbHeight = 24;
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar, GUILayout.Height(tbHeight)))
            {
                if (GUILayout.Button("Reload", EditorStyles.toolbarButton))
                {
                    ReloadCallHierarchy();
                }
                GUILayout.FlexibleSpace();
                EditorGUI.BeginChangeCheck();
                string filter = EditorGUILayout.DelayedTextField(m_TreeView.stringFilter, EditorStyles.toolbarSearchField);
                if (EditorGUI.EndChangeCheck())
                {
                    m_TreeView.SetStringFilter(filter);
                }

                Rect buttonRect = GUILayoutUtility.GetRect(52, 16);
                if (GUI.Button(buttonRect, "Filter", EditorStyles.toolbarDropDown))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Filter Selected"), false, () => {
                        m_TreeView.SetAutoFilter(false);
                        m_TreeView.SetObjectFilter(Selection.activeGameObject);
                    });
                    menu.AddItem(new GUIContent("Clear Filter"), false, () => {
                        m_TreeView.SetAutoFilter(false);
                        m_TreeView.SetObjectFilter(null);
                        m_TreeView.SetStringFilter(string.Empty);
                    });
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Automatic Filter"), m_TreeView.AutoFilter, () => {
                        m_TreeView.ToggleAutoFilter();
                    });
                    menu.DropDown(buttonRect);
                }
                if(position.width > MIN_WIDTH)
                    showPanel = GUILayout.Toggle(showPanel, EditorGUIUtility.IconContent("UnityEditor.InspectorWindow"), EditorStyles.toolbarButton);

            }
            using(new GUILayout.HorizontalScope())
            {
                Rect r = GUILayoutUtility.GetRect(position.width - PANEL_WIDTH, position.height - tbHeight);
                m_TreeView.OnGUI(r);

                if (position.width > MIN_WIDTH && showPanel)
                {
                    PanelGUI();
                }
            }
        }

        static void SetEditorFor(MonoBehaviour bh)
        {
            if (bh == null)
                s_Editor = null;

            s_Editor = UnityEditor.Editor.CreateEditor(bh);

        }

        [SerializeField]
        static UnityEditor.Editor s_Editor;
        [SerializeField]
        static Vector2 s_panelScroll;

        void PanelGUI()
        {
            using(new GUILayout.HorizontalScope())
            {
                Rect r = GUILayoutUtility.GetRect(1, 1, GUILayout.Width(1), GUILayout.ExpandHeight(true));
                EditorGUI.DrawRect(r, Color.black);
                using(new GUILayout.VerticalScope(GUILayout.Width(PANEL_WIDTH)))
                {
                    s_Editor?.DrawHeader();

                    using (new GUILayout.HorizontalScope(GUILayout.ExpandHeight(true)))
                    {
                        GUILayout.Space(16);
                        using (new GUILayout.VerticalScope())
                        {
                            s_panelScroll = GUILayout.BeginScrollView(s_panelScroll);
                            GUILayout.Space(4);
                            s_Editor?.OnInspectorGUI();
                            GUILayout.EndScrollView();
                        }
                        GUILayout.Space(4);

                        if (s_Editor is PingableEditor)
                        {
                            if ((s_Editor as PingableEditor).needRepaint)
                            {
                                Repaint();
                            }
                        }
                    }
                }
            }
        }

        void SelectItem(MonoBehaviour target)
        {
            int selected = m_TreeView.FindID(target);
            m_TreeView.SetSelection(new[] { selected }, TreeViewSelectionOptions.RevealAndFrame);
        }

        Dictionary<string, List<CallTreeNode>> nodeRoots;

        List<MonoBehaviour> erroneous;


        void ReloadCallHierarchy()
        {
            if (nodeRoots == null)
                nodeRoots = new Dictionary<string, List<CallTreeNode>>();
            else
                nodeRoots.Clear();

            erroneous = new List<MonoBehaviour>();

            AddToCategory<EventBase>("Events");
            AddToCategory<StateMachine>("State Machines");
            AddToCategory<Factory>("Factories");
            AddToCategory<SendMessageAction>("Messages");
            AddRigs();
            CollectErroneousCallables();
            m_TreeView.Reload();
        }

        void CollectErroneousCallables()
        {
            if (erroneous == null || erroneous.Count == 0)
                return;
            var root = new List<CallTreeNode>();
            nodeRoots.Add("Erroneous Callables", root);

            foreach(var callable in erroneous)
            {
                root.Add(new CallTreeNode(callable, CallTreeNodeType.Callable, callable.name));
            }
        }

        void AddErroneous(MonoBehaviour bhv)
        {
            if (!erroneous.Contains(bhv))
                erroneous.Add(bhv);
        }

        void AddRigs()
        {
            // Populate rigs
            Dictionary<Rig.UpdateMode, Dictionary<int, List<Rig>>> allRigs = new Dictionary<Rig.UpdateMode, Dictionary<int, List<Rig>>>();

            var list = FindObjectsOfType<Rig>().ToList();
            if (list.Count == 0)
                return;

            foreach(var rig in list)
            {
                if (!allRigs.ContainsKey(rig.updateMode))
                    allRigs.Add(rig.updateMode, new Dictionary<int, List<Rig>>());

                if (!allRigs[rig.updateMode].ContainsKey(rig.rigPriority))
                    allRigs[rig.updateMode].Add(rig.rigPriority, new List<Rig>());

                allRigs[rig.updateMode][rig.rigPriority].Add(rig);
            }

            // Construct tree
            nodeRoots.Add("Rigs", new List<CallTreeNode>());
            var listRoot = nodeRoots["Rigs"];
            foreach(var updateMode in allRigs.Keys)
            {
                var group = GetGroupNode($"Update Mode: {updateMode}");
                foreach(var index in allRigs[updateMode].Keys.OrderBy(o => o))
                {
                    var indexGroup = GetGroupNode($"Priority : #{index}");
                    foreach(var rig in allRigs[updateMode][index])
                    {
                        indexGroup.Children.Add(new CallTreeNode(rig, CallTreeNodeType.Rig, $"{rig.gameObject.name} ({rig.GetType().Name})"));
                    }
                    group.Children.Add(indexGroup);
                }
                listRoot.Add(group);
            }
        }

        void AddToCategory<T>(string name) where T:MonoBehaviour
        {
            var list = Resources.FindObjectsOfTypeAll<T>().ToList();

            if (list.Count > 0)
                nodeRoots.Add(name, new List<CallTreeNode>());
            else
                return;

            var listRoot = nodeRoots[name];
            foreach (var item in list)
            {
                if (item.gameObject.scene == null || !item.gameObject.scene.isLoaded)
                    continue;

                var stack = new Stack<object>();
                
                if(typeof(T) == typeof(StateMachine))
                {
                    listRoot.Add(GetStateMachineNode(item as StateMachine, stack));
                }
                else if(typeof(T) == typeof(SendMessageAction))
                {
                    listRoot.Add(GetMessageNode(item as SendMessageAction, stack));
                }
                else
                {
                    listRoot.Add(GetNode(item, stack));
                }
            }
        }

        CallTreeNode GetNode(MonoBehaviour bhv, Stack<object> stack)
        {
            if(!stack.Contains(bhv))
            {
                stack.Push(bhv);
                var rootNode = new CallTreeNode(bhv, GetType(bhv), $"{bhv.gameObject.name} ({bhv.GetType().Name})");
                var type = bhv.GetType();
                foreach (var field in type.GetFields())
                {
                    // Find Fields that are Callable[]
                    if (field.FieldType.IsAssignableFrom(typeof(Callable[])))
                    {
                        var node = new CallTreeNode(bhv, CallTreeNodeType.Callable, field.Name);
                        var value = (Callable[])field.GetValue(bhv);

                        if (value != null && value.Length > 0)
                        {
                            rootNode.Children.Add(node);
                            // Add Callables from this Callable[] array
                            foreach (var call in value)
                            {
                                if (call != null)
                                    node.Children.Add(GetCallableNode(call, stack));
                                else
                                    AddErroneous(node.Target);
                            }
                        }
                    }
                }
                return rootNode;
            }
            else
            {
                return new CallTreeNode(bhv, GetType(bhv), $"RECURSED : {bhv.gameObject.name} ({bhv.GetType().Name})");
            }
        }

        CallTreeNode GetCallableNode(Callable c, Stack<object> stack)
        {
            if (!stack.Contains(c))
            {
                stack.Push(c);
                var rootNode = new CallTreeNode(c, GetType(c), $"{c.Name} ({c.gameObject.name} : {c.GetType().Name})");
                var type = c.GetType();
                foreach (var field in type.GetFields())
                {
                    // Find Fields that are Callable[]
                    if (field.FieldType.IsAssignableFrom(typeof(Callable[])))
                    {
                        var node = new CallTreeNode(c, CallTreeNodeType.Callable, field.Name);
                        var value = (Callable[])field.GetValue(c);

                        if (value != null && value.Length > 0)
                        {
                            rootNode.Children.Add(node);
                            // Add Callables from this Callable[] array
                            foreach (var call in value)
                            {
                                if (call != null)
                                    node.Children.Add(GetCallableNode(call, stack));
                                else
                                    AddErroneous(node.Target);
                            }
                        }
                    }
                }
                return rootNode;
            }
            else
            {
                return new CallTreeNode(c, GetType(c), $"RECURSED : {c.Name} ({c.gameObject.name} : {c.GetType().Name})");
            }
        }

        CallTreeNode GetMessageNode(SendMessageAction msg, Stack<object> stack)
        {
            if (!stack.Contains(msg))
            {
                stack.Push(msg);
                var rootNode = new CallTreeNode(msg, CallTreeNodeType.Message, $"{msg.message} : ({msg.gameObject.name}.{msg.Name})");
                var all = Resources.FindObjectsOfTypeAll<OnMessageEvent>().Where(o=> o.MessageName == msg.message).ToList();

                foreach(var evt in all)
                {
                    rootNode.Children.Add(GetNode(evt, stack));
                }
                return rootNode;
            }
            else
            {
                return new CallTreeNode(msg, GetType(msg), $"RECURSED :{msg.message} : ({msg.gameObject.name}.{msg.Name})");
            }
        }

        CallTreeNode GetGroupNode(string name)
        {
            return new CallTreeNode(null, CallTreeNodeType.Group, name);
        }


        CallTreeNode GetStateMachineNode(StateMachine sm, Stack<object> stack)
        {
            if (!stack.Contains(sm))
            {
                stack.Push(sm);
                var rootNode = new CallTreeNode(sm, CallTreeNodeType.StateMachine, sm.gameObject.name);
                var type = sm.GetType();
                foreach (var field in type.GetFields())
                {
                    // Find Fields that are State[]
                    if (field.FieldType.IsAssignableFrom(typeof(State[])))
                    {
                        // Add Callables from this Callable[] array
                        var value = (State[])field.GetValue(sm);
                        foreach (var state in value)
                        {
                            if (state != null)
                                rootNode.Children.Add(GetStateNode(state, stack));
                            else
                                AddErroneous(rootNode.Target);
                        }
                    }
                }
                return rootNode;
            }
            else
            {
                return new CallTreeNode(sm, GetType(sm), $"RECURSED :{sm.gameObject.name}");
            }

        }

        CallTreeNode GetStateNode(State st, Stack<object> stack)
        {
            if (!stack.Contains(st))
            {
                stack.Push(st);
                var rootNode = new CallTreeNode(st, CallTreeNodeType.State, st.gameObject.name);
                var type = st.GetType();
                foreach (var field in type.GetFields())
                {
                    // Find Fields that are Callable[]
                    if (field.FieldType.IsAssignableFrom(typeof(Callable[])))
                    {
                        var node = new CallTreeNode(st, CallTreeNodeType.Callable, field.Name);
                        rootNode.Children.Add(node);
                        // Add Callables from this Callable[] array
                        var value = (Callable[])field.GetValue(st);
                        foreach (var call in value)
                        {
                            if (call != null)
                                node.Children.Add(GetNode(call, stack));
                            else
                                AddErroneous(rootNode.Target);
                        }
                    }
                }
                return rootNode;
            }
            else
            {
                return new CallTreeNode(st, GetType(st), $"RECURSED :{st.gameObject.name}");
            }
        }

        CallTreeNodeType GetType(MonoBehaviour bhv)
        {
            if (bhv == null)
                return CallTreeNodeType.Group;
            else if (bhv is EventBase)
                return CallTreeNodeType.Event;
            else if (bhv is LogicBase)
                return CallTreeNodeType.Logic;
            else if (bhv is ActionBase)
                return CallTreeNodeType.Action;
            else if (bhv is StateMachine)
                return CallTreeNodeType.StateMachine;
            else if (bhv is State)
                return CallTreeNodeType.State;
            else if (bhv is Factory)
                return CallTreeNodeType.Factory;
            else if (bhv is OnMessageEvent || bhv is SendMessageAction)
                return CallTreeNodeType.Message;
            else
                return CallTreeNodeType.Callable;
        }

        class CallTreeNode
        {
            public string Name;
            public MonoBehaviour Target;
            public List<CallTreeNode> Children;
            public CallTreeNodeType Type;
            public CallTreeNode(MonoBehaviour target, CallTreeNodeType type, string name = "")
            {
                Name = string.IsNullOrEmpty(name) ? target.GetType().Name : name;
                Target = target;
                Type = type;
                Children = new List<CallTreeNode>();
            }

            public bool Filter(GameObject go, string filter)
            {
                bool keep = (go == null || this.Target.gameObject == go) 
                    && (string.IsNullOrEmpty(filter) ? true : this.Name.Contains(filter));

                if(!keep)
                {
                    foreach (var node in Children)
                        keep = keep || node.Filter(go, filter);
                }

                return keep;
            }
        }

        public enum CallTreeNodeType
        {
            Callable,
            Event,
            Logic,
            Action,
            Message,
            StateMachine,
            State,
            Factory,
            Rig,
            Group,
        }

        class CallTreeView : TreeView
        {
            Dictionary<string, List<CallTreeNode>> m_Roots;
            Dictionary<int, CallTreeNode> m_Bindings;

            public CallTreeView(Dictionary<string, List<CallTreeNode>> roots) : base(new TreeViewState())
            {
                m_Roots = roots;
                m_Bindings = new Dictionary<int, CallTreeNode>();
            }

            public string stringFilter { get { return m_StringFilter; } }

            [SerializeField]
            GameObject m_filter = null;
            [SerializeField]
            string m_StringFilter = "";

            public bool AutoFilter { get; private set; }
            public void ToggleAutoFilter()
            {
                SetAutoFilter(!AutoFilter);
            }

            public void SetAutoFilter(bool value)
            {
                AutoFilter = value;
                if (AutoFilter)
                {
                    Selection.selectionChanged += UpdateAutoFilter;
                    if(this.HasSelection())
                    {
                        SetObjectFilter(m_Bindings[this.GetSelection()[0]].Target.gameObject);
                    }
                }
                else
                    Selection.selectionChanged -= UpdateAutoFilter;
            }

            void UpdateAutoFilter()
            {
                if (Selection.activeGameObject != null)
                    SetObjectFilter(Selection.activeGameObject);
            }

            public void SetObjectFilter(GameObject filter = null)
            {
                m_filter = filter;
                Reload();
            }

            public void SetStringFilter(string stringFilter)
            {
                m_StringFilter = stringFilter;
                Reload();
            }

            protected override TreeViewItem BuildRoot()
            {
                int id = -1;
                m_Bindings.Clear();
                var treeRoot = new TreeViewItem(++id, -1, "~Root");

                foreach(var kvp in m_Roots)
                {
                    if (kvp.Value == null || kvp.Value.Count == 0)
                        continue;

                    var currentRoot = new TreeViewItem(++id, 0, kvp.Key);
                    treeRoot.AddChild(currentRoot);
                    foreach (var node in kvp.Value)
                    {
                        if (node.Type != CallTreeNodeType.Group && !node.Filter(m_filter, m_StringFilter))
                            continue;

                        currentRoot.AddChild(GetNode(node, ref id, 1));
                    }
                }
                if (treeRoot.children == null)
                {
                    treeRoot.AddChild(new TreeViewItem(1, 0, "(No Results)"));
                }

                return treeRoot;
            }

            public int FindID(MonoBehaviour target)
            {
                if (m_Bindings.Any(o => o.Value.Target == target))
                    return m_Bindings.Where(o => o.Value.Target == target).First().Key;
                else
                    return int.MinValue;
            }

            TreeViewItem GetNode(CallTreeNode node, ref int id, int depth)
            {
                id++;
                var item = new TreeViewItem(id, depth, $"{node.Name}");
                item.icon = GetIcon(node.Target, node.Type);
                m_Bindings.Add(id, node);

                foreach(var child in node.Children)
                {
                    // If this is a group, filter all its direct children
                    if (node.Type == CallTreeNodeType.Group && !child.Filter(m_filter, m_StringFilter))
                        continue;

                    item.AddChild(GetNode(child, ref id, depth + 1));
                }
                return item;
            }

            Texture2D GetIcon(MonoBehaviour bhv, CallTreeNodeType type)
            {
                if(bhv != null && type != CallTreeNodeType.Callable)
                {
                    var texture = EditorGUIUtility.ObjectContent(bhv, bhv.GetType()).image;
                    if (texture != null)
                        return texture as Texture2D;
                }

                switch(type)
                {
                    default:
                    case CallTreeNodeType.Group:
                        return Styles.Group;
                    case CallTreeNodeType.Rig:
                        return Styles.Rig;
                    case CallTreeNodeType.Callable:
                        return Styles.Callable;
                    case CallTreeNodeType.Action:
                        return Styles.Action;
                    case CallTreeNodeType.Logic:
                        return Styles.Logic;
                    case CallTreeNodeType.Event:
                        return Styles.Event;
                    case CallTreeNodeType.Message:
                        return Styles.Message;
                    case CallTreeNodeType.State:
                        return Styles.State;
                    case CallTreeNodeType.Factory:
                        return Styles.Factory;
                    case CallTreeNodeType.StateMachine:
                        return Styles.StateMachine;
                }
            }

            protected override void SelectionChanged(IList<int> selectedIds)
            {
                if (AutoFilter)
                    return;

                base.SelectionChanged(selectedIds);
                if (selectedIds.Count > 0 && m_Bindings.ContainsKey(selectedIds[0]))
                    if(m_Bindings[selectedIds[0]].Target != null)
                    {
                        var node = m_Bindings[selectedIds[0]];

                        if (!(showPanel && instance.position.width > MIN_WIDTH))
                        {
                            Selection.activeObject = node.Target;

                            if (node.Type == CallTreeNodeType.Rig
                                || node.Type == CallTreeNodeType.Action
                                || node.Type == CallTreeNodeType.Logic
                                || node.Type == CallTreeNodeType.Event
                                || node.Type == CallTreeNodeType.StateMachine
                                || node.Type == CallTreeNodeType.State
                                )
                            {
                                PingableEditor.PingObject(node.Target);
                            }
                        }
                        else
                            SetEditorFor(node.Target);
                    }
            }

            public static class Styles
            {
                public static Texture2D Group = null;
                public static Texture2D Callable = Icon("Misc/ic-callable.png");
                public static Texture2D Rig = Icon("Misc/ic-callable.png");
                public static Texture2D Action = Icon("Actions/ic-action-generic.png");
                public static Texture2D Logic = Icon("Logic/ic-generic-logic.png");
                public static Texture2D Event = Icon("Events/ic-event-generic.png");
                public static Texture2D Message = Icon("Events/ic-event-message .png");
                public static Texture2D StateMachine = Icon("Misc/ic-StateMachine.png");
                public static Texture2D State = Icon("Misc/ic-State.png");
                public static Texture2D Factory = Icon("Misc/ic-Factory.png");

                static Texture2D Icon(string path)
                {
                    return AssetDatabase.LoadAssetAtPath<Texture2D>($"Packages/net.peeweek.gameplay-ingredients/Icons/{path}");
                }
            }
        }
    }
}

