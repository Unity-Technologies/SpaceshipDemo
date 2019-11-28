using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace GameplayIngredients.Editor
{
    public class DiscoverAsset : ScriptableObject
    {
        [MenuItem("Assets/Create/Discover Asset", priority = 202)]
        static void Create()
        {
            AssetFactory.CreateAssetInProjectWindow<DiscoverAsset>(null, "New DiscoverAsset.asset");
        }

        [OnOpenAsset]
        static bool OpenAsset(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);
            if (asset is DiscoverAsset)
            {
                DiscoverWindow.ShowDiscoverWindow(asset as DiscoverAsset);
                return true;
            }
            else
                return false;
        }

        [Header("General Properties")]
        public string WindowTitle = "Discover";
        public Texture2D HeaderTexture;

        [Tooltip("Width of the Window, in pixels")]
        public int WindowWidth = 640;
        [Tooltip("Height of the Window, in pixels")]
        public int WindowHeight = 520;
        [Tooltip("Width of the Discover List, in pixels")]
        public int DiscoverListWidth = 180;

        [Header("Show At Startup")]
        public bool EnableShowAtStartup = true;
        [Tooltip("The name of the preference for auto showing at startup, will be ")]
        public string PreferenceName = "Discover";

        [Header("Content")]
        public string Title = "Welcome!";
        [Multiline]
        public string Description = "This is a sample body for your discover window.";
        [Header("Scenes")]
        public DiscoverSceneInfo[] Scenes;

        [Header("Debug")]
        public bool Debug = false;
    }

    [System.Serializable]
    public struct DiscoverSceneInfo
    {
        public string Title;
        [Multiline]
        public string Description;
        public EditorSceneSetup[] SceneSetups;
        public SceneAsset[] SingleScenes;
    }
}


