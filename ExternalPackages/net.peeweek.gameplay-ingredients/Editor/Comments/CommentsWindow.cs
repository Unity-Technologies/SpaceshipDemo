using GameplayIngredients.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameplayIngredients.Comments.Editor
{
    public class CommentsWindow : EditorWindow
    {
        static CommentsWindow s_Instance;

        [MenuItem("Window/Gameplay Ingredients/Comments")]
        public static void Open()
        {
            s_Instance = EditorWindow.GetWindow<CommentsWindow>();
        }

        private void OnEnable()
        {
            titleContent = EditorGUIUtility.IconContent("console.infoicon.inactive.sml");
            titleContent.text = "Comments";
            minSize = new Vector2(680, 180);
            Refresh();
            EditorSceneManager.sceneOpened += EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneClosed += EditorSceneManager_sceneClosed;
            EditorSceneSetup.onSetupLoaded += EditorSceneSetup_onSetupLoaded;
            EditorSceneManager.sceneLoaded += SceneManager_sceneLoaded;
            EditorSceneManager.sceneUnloaded += SceneManager_sceneUnloaded;      
        }

        private void EditorSceneSetup_onSetupLoaded(EditorSceneSetup setup)
        {
            Refresh();
        }

        private void EditorSceneManager_sceneClosed(Scene scene)
        {
            Refresh();
        }

        private void EditorSceneManager_sceneOpened(Scene scene, OpenSceneMode mode)
        {
            Refresh();
        }

        private void SceneManager_sceneUnloaded(Scene arg0)
        {
            Refresh();
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Refresh();
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= EditorSceneManager_sceneOpened;
            EditorSceneManager.sceneClosed -= EditorSceneManager_sceneClosed;
            EditorSceneSetup.onSetupLoaded -= EditorSceneSetup_onSetupLoaded;
            EditorSceneManager.sceneLoaded -= SceneManager_sceneLoaded;
            EditorSceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
        }

        enum SortMode
        {
            None,
            Name,
            Description,
            Location,
            From,
            Type,
            Priority,
            State
        }

        SortMode sortMode = SortMode.None;

        public enum UserFilter
        {
            MyComments,
            AllComments,
        }

        const string kPrefixPreference = "GameplayIngredients.Comments.";
        static readonly string kUserFilterPreference = $"{kPrefixPreference}UserFilter";
        static readonly string kUserPreference = $"{kPrefixPreference}User";

        static UserFilter userFilter
        {
            get { return (UserFilter)EditorPrefs.GetInt(kUserFilterPreference, 0); }
            set { EditorPrefs.SetInt(kUserFilterPreference, (int)value); }
        }

        [InitializeOnLoadMethod]
        static void SetDefaultUser()
        {
            var user = EditorPrefs.GetString(kUserPreference, "");
            if(user == "")
            {
                user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
        }

        public static string user
        {
            get { return EditorPrefs.GetString(kUserPreference, ""); }
            set { EditorPrefs.SetString(kUserPreference, value); }
        }

        bool GetShowPref(Enum item)
        {
            string name = $"{kPrefixPreference}.Show.{item.GetType().Name}.{item}";
            return EditorPrefs.GetBool(name, true);
        }

        void SetShowPref(Enum item, bool value)
        {
            EditorPrefs.SetBool($"{kPrefixPreference}.Show.{item.GetType().Name}.{item}", value);
        }

        void ToggleShowPref(Enum item)
        {
            SetShowPref(item, !GetShowPref(item));
        }

        void MenuToggleShowPref(object item)
        {
            ToggleShowPref(item as Enum);
        }

        int highCount => results == null ? 0 : results.Count(o => o.Item1.computedPriority == CommentPriority.High);
        int mediumCount => results == null ? 0 : results.Count(o => o.Item1.computedPriority == CommentPriority.Medium);
        int lowCount => results == null ? 0 : results.Count(o => o.Item1.computedPriority == CommentPriority.Low);

        string searchFilter;
        Vector2 scrollPosition;

        bool MatchFilter(UnityEngine.Object obj, Comment comment, string filter)
        {
            filter = filter.ToLowerInvariant();
            return obj.name.Contains(filter)
                || MatchFilter(comment, filter)
                ;
        }

        bool MatchFilter(Comment comment, string filter)
        {
            return comment.title.ToLowerInvariant().Contains(filter)
                || comment.users.Any(o => o.ToLowerInvariant().Contains(filter))
                || MatchFilter(comment.message, filter)
                || comment.replies.Any(m => MatchFilter(m, filter))
                ;
        }

        bool MatchFilter(CommentMessage message, string filter)
        {
            return message.body.ToLowerInvariant().Contains(filter)
                || message.from.ToLowerInvariant().Contains(filter)
                || message.attachedObjects.Any(o => o.name.ToLowerInvariant().Contains(filter))
                || message.URL.ToLowerInvariant().Contains(filter)
                ;
        }

        private void OnGUI()
        {
            // Toolbar
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(24)))
                {
                    SceneCommentEditor.CreateComment();
                    Refresh();
                }

                if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), EditorStyles.toolbarButton, GUILayout.Width(24)))
                    Refresh();

                searchFilter = EditorGUILayout.DelayedTextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.ExpandWidth(true));
                userFilter = (UserFilter)EditorGUILayout.EnumPopup(userFilter, EditorStyles.toolbarDropDown, GUILayout.Width(128));
                if (GUILayout.Button("Type", EditorStyles.toolbarDropDown, GUILayout.Width(64)))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(CommentType.Bug.ToString()), GetShowPref(CommentType.Bug), MenuToggleShowPref, CommentType.Bug);
                    menu.AddItem(new GUIContent(CommentType.Info.ToString()), GetShowPref(CommentType.Info), MenuToggleShowPref, CommentType.Info);
                    menu.AddItem(new GUIContent(CommentType.Request.ToString()), GetShowPref(CommentType.Request), MenuToggleShowPref, CommentType.Request);
                    menu.AddItem(new GUIContent(CommentType.ToDo.ToString()), GetShowPref(CommentType.ToDo), MenuToggleShowPref, CommentType.ToDo);
                    menu.DropDown(new Rect(position.width - 240, 10, 12, 12));
                }
                if (GUILayout.Button("State", EditorStyles.toolbarDropDown, GUILayout.Width(64)))
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent(CommentState.Open.ToString()), GetShowPref(CommentState.Open), MenuToggleShowPref, CommentState.Open);
                    menu.AddItem(new GUIContent(CommentState.Resolved.ToString()), GetShowPref(CommentState.Resolved), MenuToggleShowPref, CommentState.Resolved);
                    menu.AddItem(new GUIContent(CommentState.Closed.ToString()), GetShowPref(CommentState.Closed), MenuToggleShowPref, CommentState.Closed);
                    menu.AddItem(new GUIContent(CommentState.WontFix.ToString()), GetShowPref(CommentState.WontFix), MenuToggleShowPref, CommentState.WontFix);
                    menu.AddItem(new GUIContent(CommentState.Blocked.ToString()), GetShowPref(CommentState.Blocked), MenuToggleShowPref, CommentState.Blocked);
                    menu.DropDown(new Rect(position.width - 176, 10, 12, 12));
                }
                GUILayout.Space(16);

                EditorGUI.BeginChangeCheck();
                bool showHigh = GUILayout.Toggle(GetShowPref(CommentPriority.High), CommentEditor.GetPriorityContent(highCount.ToString(), CommentPriority.High), EditorStyles.toolbarButton, GUILayout.Width(32));
                bool showMedium = GUILayout.Toggle(GetShowPref(CommentPriority.Medium), CommentEditor.GetPriorityContent(mediumCount.ToString(), CommentPriority.Medium), EditorStyles.toolbarButton, GUILayout.Width(32));
                bool showLow = GUILayout.Toggle(GetShowPref(CommentPriority.Low), CommentEditor.GetPriorityContent(lowCount.ToString(), CommentPriority.Low), EditorStyles.toolbarButton, GUILayout.Width(32));
                if(EditorGUI.EndChangeCheck())
                {
                    SetShowPref(CommentPriority.High, showHigh);
                    SetShowPref(CommentPriority.Medium, showMedium);
                    SetShowPref(CommentPriority.Low, showMedium);
                }
            }

            GUI.backgroundColor = Color.white * 1.25f;

            // Header
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                SortButton("Commment", SortMode.Name, 180);
                SortButton("Description", SortMode.Description, position.width - 541);
                SortButton("Location", SortMode.Location, 100);
                SortButton("From", SortMode.From, 80);
                SortButton("Type", SortMode.Type, 50);
                SortButton("Priority", SortMode.Priority, 60);
                SortButton("State", SortMode.State, 70);
            }
            GUI.backgroundColor = Color.white;
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);



            int i = 0;
            // Lines
            foreach (var comment in results)
            {
                if(comment.Item2 == null)
                {
                    Refresh();
                    break;
                }

                if(comment.Item2 is SceneComment)
                {

                    if (!DrawComment(comment.Item1, i, (comment.Item2 as SceneComment).gameObject))
                        continue;
                } 
                else
                {
                    if (!DrawComment(comment.Item1, i, comment.Item2 as CommentAsset))
                        continue;
                }
                i++;
            }

            EditorGUILayout.EndScrollView();
        }

        void SortButton(string label, SortMode mode, float width)
        {
            if (GUILayout.Button(label, sortMode == mode ? Styles.sortHeader : Styles.header, GUILayout.Width(width)))
            {
                if(Event.current.shift)
                    sortMode = SortMode.None;
                else if (sortMode != mode)
                        sortMode = mode;
            }
            SortResults();
        }

        bool DrawComment(Comment comment, int index, UnityEngine.Object parent)
        {
            if (comment.computedState == CommentState.Open && !GetShowPref(CommentState.Open)) return false;
            if (comment.computedState == CommentState.Resolved && !GetShowPref(CommentState.Resolved)) return false;
            if (comment.computedState == CommentState.Blocked && !GetShowPref(CommentState.Blocked)) return false;
            if (comment.computedState == CommentState.WontFix && !GetShowPref(CommentState.WontFix)) return false;
            if (comment.computedState == CommentState.Closed && !GetShowPref(CommentState.Closed)) return false;

            if (comment.computedType == CommentType.Bug && !GetShowPref(CommentType.Bug)) return false;
            if (comment.computedType == CommentType.Info && !GetShowPref(CommentType.Info)) return false;
            if (comment.computedType == CommentType.Request && !GetShowPref(CommentType.Request)) return false;
            if (comment.computedType == CommentType.ToDo && !GetShowPref(CommentType.ToDo)) return false;

            if (comment.computedPriority == CommentPriority.High && !GetShowPref(CommentPriority.High)) return false;
            if (comment.computedPriority == CommentPriority.Medium && !GetShowPref(CommentPriority.Medium)) return false;
            if (comment.computedPriority == CommentPriority.Low && !GetShowPref(CommentPriority.Low)) return false;

            if (userFilter == UserFilter.MyComments && !comment.users.Contains(user))
                return false;

            if (!string.IsNullOrEmpty(searchFilter) && !MatchFilter(parent, comment, searchFilter))
                return false;

            GUI.backgroundColor = (index % 2 == 0) ? Color.white : Color.white * 0.9f;
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button(CommentEditor.GetPriorityContent(comment.title, comment.computedPriority), Styles.line, GUILayout.Width(180)))
                    Selection.activeObject = parent;

                GUILayout.Label(comment.message.body, Styles.line, GUILayout.Width(position.width - 541));
                if(parent is GameObject)
                    GUILayout.Label((parent as GameObject).scene.name, Styles.line, GUILayout.Width(100));
                else
                    GUILayout.Label((parent as CommentAsset).name, Styles.line, GUILayout.Width(100));

                GUILayout.Label(comment.message.from, Styles.line, GUILayout.Width(80));
                GUILayout.Label(comment.computedType.ToString(), Styles.line, GUILayout.Width(50));
                GUILayout.Label(comment.computedPriority.ToString(), Styles.line, GUILayout.Width(60));
                GUILayout.Label(comment.computedState.ToString(), Styles.line, GUILayout.Width(70));
            }
            return true;
        }



        List<Tuple<Comment, UnityEngine.Object>> results;

        public static void RequestRefresh()
        {
            if (s_Instance != null)
                s_Instance.Refresh();

        }

        void Refresh()
        {
            if (results == null)
                results = new List<Tuple<Comment, UnityEngine.Object>>();
            else
                results.Clear();

            foreach(var obj in Resources.FindObjectsOfTypeAll(typeof(SceneComment)))
            {
                results.Add(new Tuple<Comment, UnityEngine.Object>((obj as SceneComment).comment, obj));
            }
            foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(CommentAsset).Name}"))
            {
                CommentAsset ca = AssetDatabase.LoadAssetAtPath<CommentAsset>(AssetDatabase.GUIDToAssetPath(guid));
                results.Add(new Tuple<Comment, UnityEngine.Object>(ca.comment, ca));
            }
            SortResults();
            Repaint();
        }

        void SortResults()
        {
            if (results == null)
                return;
            switch (sortMode)
            {
                case SortMode.None:
                    break;
                case SortMode.Name:
                    results = results.OrderBy(o => o.Item2.name).ToList();
                    break;
                case SortMode.Description:
                    results = results.OrderBy(o => o.Item1.title).ToList();
                    break;
                case SortMode.Location:
                    results = results.OrderBy(o => o.Item2 is GameObject ? (o.Item2 as GameObject).scene.name : o.Item2.name ).ToList();
                    break;
                case SortMode.From:
                    results = results.OrderBy(o => o.Item1.message.from).ToList();
                    break;
                case SortMode.Type:
                    results = results.OrderBy(o => o.Item1.computedType).ToList();
                    break;
                case SortMode.Priority:
                    results = results.OrderBy(o => o.Item1.computedPriority).ToList();
                    break;
                case SortMode.State:
                    results = results.OrderBy(o => o.Item1.computedState).ToList();
                    break;
                default:
                    break;
            }
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
                line.padding = new RectOffset();
                line.contentOffset = new Vector2(6, 2);
                line.alignment = TextAnchor.UpperLeft;
                line.wordWrap = false;
            }
        }


        [SettingsProvider]
        public static SettingsProvider CommentsPreferences()
        {
            return new SettingsProvider("Preferences/Gameplay Ingredients/Comments", SettingsScope.User)
            {
                label = "Comments",
                guiHandler = (searchContext) =>
                {
                    user = EditorGUILayout.DelayedTextField("Project User Nickname", user);
                }
            };


        }

    }
}
