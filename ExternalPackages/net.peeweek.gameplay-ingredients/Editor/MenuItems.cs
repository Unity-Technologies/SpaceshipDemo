using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace GameplayIngredients.Editor
{
    public static class MenuItems
    {
        public const int kWindowMenuPriority = 100;
        public const int kPlayMenuPriority = 160;
        public const int kMenuPriority = 330;

        #region PLAY HERE

        [MenuItem("Edit/Play from SceneView Position #%&P", priority = kPlayMenuPriority)]
        static void PlayHere()
        {
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Edit/Play from SceneView Position #%&P", priority = kPlayMenuPriority, validate = true)]
        static bool PlayHereValidate()
        {
            return PlayFromHere.IsReady;
        }

        #endregion

        #region GROUP_UNGROUP

        const int kGroupMenuIndex = 500;
        const string kGroupMenuString = "Edit/Group Selected %G";
        const string kUnGroupMenuString = "Edit/Un-Group Selected %#G";

        [MenuItem(kGroupMenuString, priority = kGroupMenuIndex, validate = false)]
        static void Group()
        {
            if (Selection.gameObjects.Length <= 1)
                return;

            var selected = Selection.gameObjects;
            Transform parent = selected[0].transform.parent;
            Scene scene = selected[0].scene;

            bool sparseParents = false;

            foreach (var obj in selected)
            {
                if (obj.transform.parent != parent || obj.scene != scene)
                {
                    sparseParents = true;
                    break;
                }
            }

            if (sparseParents)
            {
                parent = null;
                scene = SceneManager.GetActiveScene();
            }

            Vector3 posSum = Vector3.zero;

            foreach (var go in selected)
            {
                posSum += go.transform.position;
            }

            GameObject groupObj = new GameObject("Group");
            groupObj.transform.position = posSum / selected.Length;
            groupObj.transform.parent = parent;
            groupObj.isStatic = true;

            foreach (var go in selected)
                go.transform.parent = groupObj.transform;

            // Expand by pinging the first object
            EditorGUIUtility.PingObject(selected[0]);
            
        }

        [MenuItem(kGroupMenuString, priority = kGroupMenuIndex, validate = true)]
        static bool GroupCheck()
        {
            return (Selection.gameObjects.Length > 1);
        }


        [MenuItem(kUnGroupMenuString, priority = kGroupMenuIndex+1, validate = false)]
        static void UnGroup()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var selected = Selection.gameObjects;
            List<Transform> oldParents = new List<Transform>();
            foreach(var go in selected)
            {
                if(go.transform.parent != null)
                {
                    if(!oldParents.Contains(go.transform.parent))
                        oldParents.Add(go.transform.parent);

                    go.transform.parent = go.transform.parent.parent;
                }
            }

            List<GameObject> toDelete = new List<GameObject>();

            // Cleanup old parents
            foreach(var parent in oldParents)
            {
                var go = parent.gameObject;
                if(parent.childCount == 0 && parent.GetComponents<Component>().Length == 1) // if no more children and only transform/rectTransform
                {
                    toDelete.Add(go);
                }
            }

            foreach (var trash in toDelete)
                GameObject.DestroyImmediate(trash);
            
        }

        [MenuItem(kUnGroupMenuString, priority = kGroupMenuIndex+1, validate = true)]
        static bool UnGroupCheck()
        {
            return (Selection.gameObjects.Length > 0);
        }

        #endregion

        #region ASSETS

        [UnityEditor.MenuItem("Assets/Create/Game Level")]
        static void CreateGameLevel()
        {
            GameplayIngredients.Editor.AssetFactory.CreateAssetInProjectWindow<GameLevel>("", "New Game Level.asset");
        }

        #endregion

        #region HELP
        static string helpBaseURL => Help.URL;

        static void OpenHelp(string page)
        {
            Application.OpenURL($"{helpBaseURL}{page}/");
        }

        static void OpenHelp()
        {
            Application.OpenURL(helpBaseURL);
        }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Table of Contents")]
        static void Help_StartPage() { OpenHelp(); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Installing")]
        static void Help_Install() { OpenHelp("install"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Settings Asset")]
        static void Help_Settings() { OpenHelp("settings"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Version Compatibility")]
        static void Help_Versions() { OpenHelp("versions"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Getting Involved")]
        static void Help_Contribute() { OpenHelp("engage"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Events,Logic and Actions")]
        static void Help_EventLogicActions() { OpenHelp("events-logic-actions"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Callables")]
        static void Help_Callables() { OpenHelp("callable"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Managers")]
        static void Help_Managers() { OpenHelp("managers"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Messager")]
        static void Help_Messager() { OpenHelp("messager"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Rigs")]
        static void Help_Rigs() { OpenHelp("rigs"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/State Machines")]
        static void Help_StateMachines() { OpenHelp("state-machines"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Factories")]
        static void Help_Factories() { OpenHelp("factories"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Timers")]
        static void Help_Timers() { OpenHelp("timers"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Global and Local Variables")]
        static void Help_Globals() { OpenHelp("globals"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Counters")]
        static void Help_Counters() { OpenHelp("counters"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Runtime/Interactive")]
        static void Help_Interactive() { OpenHelp("interactive"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Welcome Screen")]
        static void Help_WelcomeScreen() { OpenHelp("welcome-screen"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Play from Here")]
        static void Help_PlayFromHere() { OpenHelp("play-from-here"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Advanced Hierarchy View")]
        static void Help_AdvHierarchy() { OpenHelp("hierarchy-hints"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Link Game View")]
        static void Help_LinkGameView() { OpenHelp("link-game-view"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Scene Point-of-Views")]
        static void Help_ScenePOV() { OpenHelp("scene-pov"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Scene Setups")]
        static void Help_SceneSetups() { OpenHelp("scene-setups"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Find and Replace")]
        static void Help_FindReplace() { OpenHelp("find-and-replace"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Callable tree Explorer")]
        static void Help_CallableTreeExplorer() { OpenHelp("callable-tree-explorer"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Folders")]
        static void Help_Folders() { OpenHelp("folders"); }

        [MenuItem("Help/Gameplay Ingredients/Documentation/Editor/Discover")]
        static void Help_Discover() { OpenHelp("discover"); }


        [MenuItem("Help/Gameplay Ingredients/GitHub Repository (Issues and Releases)")]
        static void GitHub()
        {
            Application.OpenURL("https://github.com/peeweek/net.peeweek.gameplay-ingredients/");
        }

        [MenuItem("Help/Gameplay Ingredients/OpenUPM page")]
        static void OpenUPM()
        {
            Application.OpenURL("https://openupm.com/packages/net.peeweek.gameplay-ingredients/");
        }
        #endregion
    }
}
