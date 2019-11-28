using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace GameplayIngredients.Editor
{
    public class NewSceneWindow : EditorWindow
    {
        const int WindowWidth = 640;
        const int WindowHeight = 380;

        [MenuItem("File/New Scene From Template... &%N", priority = 150)]
        static void ShowNewSceneWindow()
        {
            GetWindow<NewSceneWindow>(true);
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("New Scene From Template");
            this.position = new Rect((Screen.width / 2.0f) - WindowWidth / 2, (Screen.height / 2.0f) - WindowHeight / 2, WindowWidth, WindowHeight);
            this.minSize = new Vector2(WindowWidth, WindowHeight);
            this.maxSize = new Vector2(WindowWidth, WindowHeight);
            ReloadList();
        }

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(8);
                GUILayout.Label("Available Templates", EditorStyles.boldLabel, GUILayout.Width(188));
                GUILayout.Label("Description", EditorStyles.boldLabel);
            }

            using (new GUILayout.HorizontalScope(GUILayout.ExpandHeight(true)))
            {
                GUILayout.Space(8);
                using (new GUILayout.VerticalScope(Styles.listBox, GUILayout.Width(180)))
                {
                    DrawTemplateList();
                }
                GUILayout.Space(4);
                templateDetailsScroll = EditorGUILayout.BeginScrollView(templateDetailsScroll);
                using (new GUILayout.VerticalScope(Styles.detailBox))
                {
                    DrawTemplateDetails();
                }
                EditorGUILayout.EndScrollView();
                GUILayout.Space(4);
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                EditorGUI.BeginDisabledGroup(selectedTemplate.Scene == null);


                if (GUILayout.Button("  Create  ", Styles.buttonLeft))
                {
                    // Creates a new map and replaces the current setup
                    CreateSceneFromTemplate(selectedTemplate.Scene);
                    this.Close();
                }

                if (GUILayout.Button("  Append  ", Styles.buttonRight))
                {
                    bool canCreateScene = false;

                    if (EditorSceneManager.GetActiveScene().path == string.Empty)
                    {
                        // We need to save active scene First
                        if(EditorUtility.DisplayDialog("Create new Scene", "You can only append a new scene if the current active scene has been saved, do you want to save it first?","Yes","No"))
                        {
                            string path = EditorUtility.SaveFilePanelInProject("Save Scene", "New Scene", "unity", "New Scene");
                            if(path != string.Empty)
                            {
                                if(EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path))
                                    canCreateScene = true;
                            }
                        }
                    }

                    if(canCreateScene)
                    {
                        // Creates a new map that is added to the current setup
                        CreateSceneFromTemplate(selectedTemplate.Scene,true);
                        this.Close();
                    }
                }

                EditorGUI.EndDisabledGroup();
                GUILayout.Space(4);
            }
            GUILayout.Space(8);
        }

        static void CreateSceneFromTemplate(SceneAsset template, bool append = false)
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, append? NewSceneMode.Additive: NewSceneMode.Single);
            var temp = EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(template), OpenSceneMode.Additive);
            var objects = temp.GetRootGameObjects();
            foreach(var obj in objects)
            {
                SceneManager.MoveGameObjectToScene(obj, newScene);
            }
            EditorSceneManager.CloseScene(temp, true);
            
        }

        public static void ReloadList()
        {
            if (templateLists == null)
                templateLists = new List<SceneTemplateList>();
            else
                templateLists.Clear();

            string[] all = AssetDatabase.FindAssets("t:" + typeof(SceneTemplateList).Name);
            foreach(var guid in all)
            {
                templateLists.Add(AssetDatabase.LoadAssetAtPath<SceneTemplateList>(AssetDatabase.GUIDToAssetPath(guid)));
            }
        }

        static List<SceneTemplateList> templateLists;
        static SceneWindowTemplate selectedTemplate;

        Vector2 scrollList = Vector2.zero;

        void DrawTemplateList()
        {
            GUILayout.BeginScrollView(scrollList);
            int i = 0;
            foreach(var list in templateLists)
            {
                if(i > 0)
                {
                    GUILayout.Space(8);
                }

                GUILayout.Label(list.ListName, Styles.listHeader);
                EditorGUI.indentLevel++;
                foreach (var template in list.Templates)
                {
                    if(GUILayout.Button(template.Name, Styles.listItem))
                    {
                        selectedTemplate = template;
                    }
                    if(template.Name == selectedTemplate.Name && template.Scene == selectedTemplate.Scene)
                    {
                        Rect hl = GUILayoutUtility.GetLastRect();
                        EditorGUI.DrawRect(hl, new Color(1, 1, 1, 0.05f));
                    }
                }
                EditorGUI.indentLevel--;
                i++;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
        }

        Vector2 templateDetailsScroll;

        void DrawTemplateDetails()
        {
            if(selectedTemplate.Scene != null)
            {
                GUILayout.Label(selectedTemplate.Name, Styles.detailsTitle);
                GUILayout.Space(16);
                GUILayout.Label(selectedTemplate.Description, Styles.detailsBody);
            }
            else
            {
                GUILayout.Label("No Template Selected");
            }

            GUILayout.FlexibleSpace();

            if(selectedTemplate.ScreenShot != null)
            {
                GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(180));
                Rect r = GUILayoutUtility.GetLastRect();
                RectOffset off = new RectOffset(6, 6, 6, 6);
                r = off.Remove(r);
                EditorGUI.DrawPreviewTexture(r, selectedTemplate.ScreenShot, null, ScaleMode.ScaleAndCrop);
            }
        }

        static class Styles
        {
            public static GUIStyle buttonLeft;
            public static GUIStyle buttonMid;
            public static GUIStyle buttonRight;

            public static GUIStyle listBox;
            public static GUIStyle listHeader;
            public static GUIStyle listItem;
            public static GUIStyle detailBox;

            public static GUIStyle detailsTitle;
            public static GUIStyle detailsBody;

            static Styles()
            {
                buttonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
                buttonMid = new GUIStyle(EditorStyles.miniButtonMid);
                buttonRight = new GUIStyle(EditorStyles.miniButtonRight);
                buttonLeft.fontSize = 12;
                buttonMid.fontSize = 12;
                buttonRight.fontSize = 12;

                listHeader = new GUIStyle("ShurikenModuleTitle");
                listHeader.padding= new RectOffset(0,0,2,2);
                listHeader.fontSize = 12;
                listHeader.stretchWidth = false;
                listHeader.fixedWidth = 172;
                listHeader.stretchHeight = true;
                listHeader.fixedHeight = 24;
                
                listBox = new GUIStyle(EditorStyles.helpBox);
                detailBox = new GUIStyle(listBox);
                detailBox.padding = new RectOffset(12, 12, 12, 12);

                listItem = new GUIStyle(EditorStyles.label);
                listItem.padding = new RectOffset(16, 0, 1, 1);

                detailsTitle = new GUIStyle(EditorStyles.label);
                detailsTitle.fontSize = 24;

                detailsBody = new GUIStyle(EditorStyles.wordWrappedLabel);
                detailsBody.fontSize = 14;

            }
        }
    }
}
