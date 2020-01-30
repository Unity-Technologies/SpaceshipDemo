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
    }
}
