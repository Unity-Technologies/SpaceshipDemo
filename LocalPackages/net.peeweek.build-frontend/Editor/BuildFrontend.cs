using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildFrontend : EditorWindow
{
    public const int CreateAssetMenuPriority = 5801;
    public const int WindowMenuPriority = 203;

    [MenuItem("File/Build Frontend %&B", priority = WindowMenuPriority)]

    static void OpenWindow()
    {
        var window = GetWindow<BuildFrontend>();
        window.PopulateAssets();
    }

    private void OnEnable()
    {
        titleContent = Contents.windowTitle;
        PopulateAssets();
    }
    
    [SerializeField]
    Dictionary<BuildTemplate, BuildReport> Reports = new Dictionary<BuildTemplate, BuildReport>();
    string reportText = string.Empty;

    private void OnGUI()
    {
        System.Action nextAction = null;

        using(new GUILayout.HorizontalScope(GUILayout.Height(88)))
        {
            var rect = GUILayoutUtility.GetRect(88,88, Styles.Icon, GUILayout.Width(88));
            GUI.DrawTexture(rect, Contents.icon);
            using(new GUILayout.VerticalScope())
            {
                GUILayout.Space(8);
                GUILayout.Label(Contents.title, Styles.Title);
                GUILayout.FlexibleSpace();
                DrawProgressBar();
                GUILayout.Space(8);
            }
            GUILayout.Space(8);

            using (new GUILayout.VerticalScope(GUILayout.Width(116)))
            {
                GUILayout.Space(8);
                if(GUILayout.Button("Build All", Styles.BuildButton, GUILayout.Height(32)))
                {
                    // Run Build
                    nextAction = DoAllBuild;
                }

                BuildTemplate template = (Selection.activeObject as BuildTemplate);
                GUILayout.Space(2);
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUI.BeginDisabledGroup(template == null);
                    if (GUILayout.Button("Build Selected", Styles.MiniButtonLeft))
                    {
                        nextAction = () =>
                        {
                            var report = template.DoBuild();
                            if (report != null)
                                Reports[template] = report;

                            m_SelectedReport = report;
                            Repaint();
                        };
                    }
                    if(GUILayout.Button("+ Run", Styles.MiniButtonRight, GUILayout.Width(48)))
                    {
                        nextAction = () =>
                        {
                            var report = template.DoBuild(true);
                            if (report != null)
                                Reports[template] = report;

                            m_SelectedReport = report;
                            Repaint();
                        };
                    }
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(template == null || !template.canRunBuild);
                if (GUILayout.Button("Run Last Build", Styles.MiniButton))
                {
                    nextAction = () =>
                    {
                        template.RunBuild();
                        EditorUtility.ClearProgressBar();
                        Repaint();
                    };
                }
                EditorGUI.EndDisabledGroup();

            }
            GUILayout.Space(8);

        }

        using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            if(GUILayout.Button("Refresh", EditorStyles.toolbarButton))
            {
                PopulateAssets();
            }
            GUILayout.FlexibleSpace();
        }

        using (new GUILayout.HorizontalScope())
        {
            DrawTemplateList();
            DrawReport();
        }

        if (nextAction != null)
        {
            var selected = Selection.activeObject;
            nextAction.Invoke();
            Selection.activeObject = selected;
        }

    }

    void DrawReport()
    {
        reportScroll = EditorGUILayout.BeginScrollView(reportScroll, Styles.scrollView);
        if (m_SelectedReport != null && Reports != null && Reports.ContainsValue(m_SelectedReport))
        {
            var template = Reports.First(o => o.Value == m_SelectedReport).Key;
            FormatReportGUI(template, m_SelectedReport);
        }
        else
        {
            EditorGUILayout.HelpBox("No Build report has been generated yet, please build this template first", MessageType.Info);
        }
        EditorGUILayout.EndScrollView();

    }

    void DrawProgressBar()
    {
        GUI.backgroundColor = Color.gray;
        using(new GUILayout.HorizontalScope(Styles.progressBarItem, GUILayout.Height(24), GUILayout.ExpandWidth(true)))
        {
            foreach(var cat in m_BuildTemplates)
            {
                foreach(var template in cat.Value)
                {
                    GUI.backgroundColor = new Color(.3f,.3f,.3f,1.0f);
                    GUI.contentColor = Color.white;
                    if(!template.BuildEnabled)
                    {
                        GUI.backgroundColor = new Color(.4f,.4f,.4f,1.0f);
                        GUI.contentColor = new Color(1.0f,1.0f,1.0f, 0.5f);
                    }
                    else if(Reports.ContainsKey(template) && Reports[template] != null)
                    {
                        var report = Reports[template];
                        switch(report.summary.result)
                        {
                            case BuildResult.Succeeded:
                                GUI.backgroundColor = new Color(.15f,.5f,.05f,1.0f);
                                GUI.contentColor = new Color(0.3f,1.0f,0.1f, 1.0f);                                
                                break;
                            case BuildResult.Cancelled:
                            case BuildResult.Unknown:
                            case BuildResult.Failed:
                                GUI.backgroundColor = new Color(.5f,.05f,.15f,1.0f);
                                GUI.contentColor = new Color(1.0f,0.1f,0.3f, 1.0f);                                
                                break;
                        }

                    }
                
                    GUILayout.Label(template.name, Styles.progressBarItem, GUILayout.ExpandHeight(true));
                }
            }
        }
        GUI.contentColor = Color.white;
        GUI.backgroundColor = Color.white;    
    }

    void DoAllBuild()
    {
        try
        {
            foreach(var cat in m_BuildTemplates)
            {
                foreach(var template in cat.Value)
                {
                    EditorUtility.DisplayProgressBar("Build Frontend", $"Building {template.name} ...", 1.0f);
                    if(template.BuildEnabled)
                    {
                        var Report = template.DoBuild();
                        Reports[template] = Report;
                        m_SelectedReport = Report;
                        Repaint();
                    }
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    Vector2 templateScroll = Vector2.zero;
    Vector2 reportScroll = Vector2.zero;
    BuildReport m_SelectedReport;

    void DrawTemplateList()
    {
        templateScroll = GUILayout.BeginScrollView(templateScroll, false, true, GUILayout.Width(240));
        
        using (new GUILayout.VerticalScope(EditorStyles.label))
        {
            foreach (var catKVP in m_BuildTemplates)
            {
                EditorGUILayout.LabelField(catKVP.Key == string.Empty? "General": catKVP.Key, EditorStyles.boldLabel);

                foreach (var template in catKVP.Value)
                {
                    // Draw Selected background box
                    if(template == CurrentTemplate)
                    {
                        Rect r = GUILayoutUtility.GetLastRect();
                        Vector2 pos = r.position;
                        pos.y += 18;
                        r.position = pos;
                        r.height += 2;
                        float gray = EditorGUIUtility.isProSkin? 1: 0;
                        EditorGUI.DrawRect(r, new Color(gray, gray, gray, 0.1f));
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(16);

                        EditorGUI.BeginChangeCheck();
                        var enabled = GUILayout.Toggle(template.BuildEnabled,GUIContent.none, GUILayout.Width(24));
                        if(EditorGUI.EndChangeCheck())
                        {
                            template.BuildEnabled = enabled;
                            EditorUtility.SetDirty(template);
                        }

                        if (GUILayout.Button(template.Name != null && template.Name != string.Empty ? template.Name : template.name, template == CurrentTemplate? Styles.SelectedProfile : EditorStyles.label))
                        {
                            if (Reports.ContainsKey(template) && Reports[template] != null)
                            {
                                m_SelectedReport = Reports[template]; 
                            }
                            else
                            {
                                m_SelectedReport = null;
                                GUILayout.Label("Build has not been run yet");
                            }

                            CurrentTemplate = template;
                            CurrentProfile = CurrentTemplate.Profile;
                            CurrentSceneList = CurrentTemplate.SceneList;
                            Selection.activeObject = template;
                        }
                    }


                }
                GUILayout.Space(16);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    [SerializeField]
    BuildTemplate CurrentTemplate;
    [SerializeField]
    BuildProfile CurrentProfile;
    [SerializeField]
    SceneList CurrentSceneList;

    GenericMenu TemplateMenu;
    GenericMenu ProfileMenu;
    GenericMenu SceneListMenu;

    Dictionary<string, List<BuildTemplate>> m_BuildTemplates;
    List<BuildProfile> m_BuildProfiles;
    List<SceneList> m_SceneLists;

    void PopulateAssets()
    {
        var buildTemplates = AssetDatabase.FindAssets("t:BuildTemplate");
        var buildProfiles = AssetDatabase.FindAssets("t:BuildProfile");
        var sceneLists = AssetDatabase.FindAssets("t:SceneList");

        m_BuildTemplates = new Dictionary<string, List<BuildTemplate>>();
        m_BuildProfiles = new List<BuildProfile>();
        m_SceneLists = new List<SceneList>();

        TemplateMenu = new GenericMenu();
        foreach (var templateGUID in buildTemplates)
        {
            string templatePath = AssetDatabase.GUIDToAssetPath(templateGUID);
            BuildTemplate template = (BuildTemplate)AssetDatabase.LoadAssetAtPath(templatePath, typeof(BuildTemplate));
            if (!m_BuildTemplates.ContainsKey(template.Category))
                m_BuildTemplates.Add(template.Category, new List<BuildTemplate>());

            m_BuildTemplates[template.Category].Add(template);

            TemplateMenu.AddItem(new GUIContent(template.MenuEntry), false, MenuSetTemplate, template);
        }

        ProfileMenu = new GenericMenu();
        foreach (var profileGUID in buildProfiles)
        {
            string profilePath = AssetDatabase.GUIDToAssetPath(profileGUID);
            BuildProfile profile = (BuildProfile)AssetDatabase.LoadAssetAtPath(profilePath, typeof(BuildProfile));
            m_BuildProfiles.Add(profile);
            ProfileMenu.AddItem(new GUIContent(profile.MenuEntry), false, MenuSetProfile, profile);
        }

        SceneListMenu = new GenericMenu();
        foreach (var sceneListGUID in sceneLists)
        {
            string sceneListPath = AssetDatabase.GUIDToAssetPath(sceneListGUID);
            SceneList sceneList = (SceneList)AssetDatabase.LoadAssetAtPath(sceneListPath, typeof(SceneList));
            m_SceneLists.Add(sceneList);
            SceneListMenu.AddItem(new GUIContent(sceneList.MenuEntry), false, MenuSetSceneList, sceneList);
        }
    }

    void MenuSetTemplate(object o)
    {
        CurrentTemplate = (BuildTemplate)o;
        CurrentProfile = CurrentTemplate.Profile;
        CurrentSceneList = CurrentTemplate.SceneList;
    }

    void MenuSetProfile(object o)
    {
        CurrentProfile = (BuildProfile)o;
        if(CurrentTemplate != null && !CurrentTemplate.name.EndsWith("*"))
        {
            CurrentTemplate = Instantiate<BuildTemplate>(CurrentTemplate) as BuildTemplate;
            CurrentTemplate.name += "*";
        }

        CurrentTemplate.Profile = CurrentProfile;
    }

    void MenuSetSceneList(object o)
    {
        CurrentSceneList = (SceneList)o;
        if (CurrentTemplate != null && !CurrentTemplate.name.EndsWith("*"))
        {
            CurrentTemplate = Instantiate<BuildTemplate>(CurrentTemplate) as BuildTemplate;
            CurrentTemplate.name += "*";
        }

        CurrentTemplate.SceneList = CurrentSceneList;
    }

    string FormatSize(ulong byteSize)
    {
        double size = byteSize;
        if (size < 1024) // Bytes
        {
            return $"{size.ToString("F2")} bytes";
        }
        else if (size < 1024 * 1024) // KiloBytes
        {
            return $"{(size / 1024).ToString("F2")} KiB ({byteSize} bytes)";
        }
        else if (size < 1024 * 1024 * 1024) // Megabytes
        {
            return $"{(size / (1024*1024)).ToString("F2")} MiB ({byteSize} bytes)";
        }
        else // Gigabytes
        {
            return $"{(size / (1024 * 1024 * 1024)).ToString("F2")} GiB ({byteSize} bytes)";
        }
    }

    void FormatReportGUI(BuildTemplate template, BuildReport report)
    {
        var summary = report.summary;

        if(template != null)
            GUILayout.Label($"{(template.Name == string.Empty? template.name : template.Name)} {(template.Category == string.Empty?"":$"({template.Category})")}", Styles.boldLabelLarge);

        using (new GUILayout.HorizontalScope())
        {
            if (summary.result == BuildResult.Succeeded)
                GUILayout.Label(Contents.buildSucceeded, GUILayout.Width(32));
            else if (summary.result != BuildResult.Unknown)
                GUILayout.Label(Contents.buildFailed, GUILayout.Width(32));

            GUILayout.Label(summary.result.ToString(), Styles.boldLabelLarge);
        }

        GUILayout.Space(8);
        GUILayout.Label("Total Build Time :" + summary.totalTime);
        EditorGUILayout.LabelField($"Build Size : {FormatSize(summary.totalSize)} ");

        if(summary.totalErrors > 0)
        {
            GUILayout.Label(new GUIContent($"{(int)summary.totalErrors} Errors", Contents.errorIconSmall.image));
        }

        if(summary.totalWarnings > 0)
        {
            GUILayout.Label(new GUIContent($"{(int)summary.totalWarnings} Warnings", Contents.warnIconSmall.image));
        }

        EditorGUILayout.TextField("Output Path", summary.outputPath);
        
        if(report.strippingInfo != null)
        {
            GUILayout.Space(8);
            
            GUILayout.Label("Included Modules", EditorStyles.boldLabel);
            var modules = report.strippingInfo.includedModules;
            foreach(var module in modules)
            {
                EditorGUILayout.LabelField(module, EditorStyles.foldout);
            }
        }

        GUILayout.Space(8);
        GUILayout.Label("Build Steps", EditorStyles.boldLabel);
        var steps = report.steps;
        foreach(var step in steps)
        {
            string prefName = $"BuildFrontend.Foldout'{step.name}'";

            using (new GUILayout.HorizontalScope())
            {
                if (step.messages.Any(o => o.type == LogType.Error || o.type == LogType.Assert || o.type == LogType.Exception))
                    GUILayout.Label(Contents.errorIconSmall, Styles.Icon,  GUILayout.Width(16));
                else if (step.messages.Any(o => o.type == LogType.Warning))
                    GUILayout.Label(Contents.warnIconSmall, Styles.Icon, GUILayout.Width(16));
                else
                    GUILayout.Label(Contents.successIcon, Styles.Icon, GUILayout.Width(16));

                if (step.messages.Length > 0)
                {
                    bool pref = EditorPrefs.GetBool(prefName, false);

                    bool newPref = EditorGUILayout.Foldout(pref, step.name);

                    if (GUI.changed && pref != newPref)
                        EditorPrefs.SetBool(prefName, newPref);
                }
                else
                {
                    EditorGUILayout.LabelField(step.name);
                }

            }

            EditorGUI.indentLevel++;
            if (EditorPrefs.GetBool(prefName, false))
            {
                foreach (var message in step.messages)
                {
                    MessageType type = MessageType.Error;
                    if (message.type == LogType.Log)
                        type = MessageType.Info;
                    else if (message.type == LogType.Warning)
                        type = MessageType.Warning;

                    EditorGUILayout.HelpBox(message.content, type, true);
                }
            }
            EditorGUI.indentLevel--;

            GUILayout.Space(2);
        }
        GUILayout.Space(128);
    }

    static class Styles
    {
        public static GUIStyle BuildButton;
        public static GUIStyle MiniButton;
        public static GUIStyle MiniButtonLeft;
        public static GUIStyle MiniButtonRight;
        public static GUIStyle progressBarItem;
        public static GUIStyle SelectedProfile;
        public static GUIStyle Title;
        public static GUIStyle Icon;
        public static GUIStyle boldLabelLarge;
        public static GUIStyle scrollView;

        static Styles()
        {
            BuildButton = new GUIStyle(EditorStyles.miniButton);
            BuildButton.fontSize = 14;
            BuildButton.fixedHeight = 32;

            MiniButton = new GUIStyle(EditorStyles.miniButton);
            MiniButton.fontSize = 10;

            MiniButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
            MiniButtonLeft.fontSize = 10;

            MiniButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
            MiniButtonRight.fontSize = 10;


            SelectedProfile = new GUIStyle(EditorStyles.label);
            var pink = EditorGUIUtility.isProSkin? new Color(1.0f, 0.2f, 0.5f, 1.0f) : new Color(1.0f, 0.05f, 0.4f, 1.0f);
            SelectedProfile.active.textColor = pink;
            SelectedProfile.focused.textColor = pink;
            SelectedProfile.hover.textColor = pink;
            SelectedProfile.normal.textColor = pink;

            Title = new GUIStyle(EditorStyles.label);
            Title.fontSize = 18;

            Icon = new GUIStyle(EditorStyles.label);

            Icon.padding = new RectOffset();
            Icon.margin = new RectOffset();

            progressBarItem = new GUIStyle(EditorStyles.miniLabel);
            progressBarItem.alignment = TextAnchor.MiddleCenter;
            progressBarItem.margin = new RectOffset(0,0,0,0);
            progressBarItem.padding = new RectOffset(0,0,0,0);
            progressBarItem.wordWrap = true;
            progressBarItem.onActive.background = Texture2D.whiteTexture;
            progressBarItem.onFocused.background = Texture2D.whiteTexture;
            progressBarItem.onHover.background = Texture2D.whiteTexture;
            progressBarItem.onNormal.background = Texture2D.whiteTexture;
            progressBarItem.active.background = Texture2D.whiteTexture;
            progressBarItem.focused.background = Texture2D.whiteTexture;
            progressBarItem.hover.background = Texture2D.whiteTexture;
            progressBarItem.normal.background = Texture2D.whiteTexture;
            
            boldLabelLarge = new GUIStyle(EditorStyles.boldLabel);
            boldLabelLarge.fontSize = 16;

            scrollView = new GUIStyle();
            scrollView.padding = new RectOffset(8, 8, 8, 8);

        }
    }
    static class Contents
    {
        public static GUIContent windowTitle; 
        public static GUIContent title; 
        public static GUIContent build = new GUIContent("Build");
        public static GUIContent template = new GUIContent("Template:");
        public static GUIContent profile = new GUIContent("Profile:");
        public static GUIContent sceneList = new GUIContent("Scene List:");
        public static Texture icon;

        public static GUIContent buildSucceeded = EditorGUIUtility.IconContent("Collab.BuildSucceeded");
        public static GUIContent buildFailed = EditorGUIUtility.IconContent("Collab.BuildFailed");

        public static GUIContent successIcon = EditorGUIUtility.IconContent("Collab");
        public static GUIContent failIcon = EditorGUIUtility.IconContent("CollabError");

        public static GUIContent errorIcon = EditorGUIUtility.IconContent("console.erroricon");
        public static GUIContent errorIconSmall = EditorGUIUtility.IconContent("console.erroricon.sml");

        public static GUIContent infoIcon = EditorGUIUtility.IconContent("console.infoicon");
        public static GUIContent infoIconSmall = EditorGUIUtility.IconContent("console.infoicon.sml");

        public static GUIContent warnIcon = EditorGUIUtility.IconContent("console.warnicon");
        public static GUIContent warnIconSmall = EditorGUIUtility.IconContent("console.warnicon.sml");

        static Contents()
        {
            icon = AssetDatabase.LoadAssetAtPath<Texture>("Packages/net.peeweek.build-frontend/Editor/Icons/BuildFrontend.png");
            var titleIcon = AssetDatabase.LoadAssetAtPath<Texture>($"Packages/net.peeweek.build-frontend/Editor/Icons/BuildFrontendTab{(EditorGUIUtility.isProSkin?"":"Personal")}.png");
            windowTitle = new GUIContent("Build Frontend", titleIcon);
            title = new GUIContent("Build Frontend");
        }

    }
}
