using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameplayIngredients.Editor
{
    public class SceneTemplateList : ScriptableObject
    {
        [MenuItem("Assets/Create/Scene Template List", priority = 201)]
        static void CreateSceneTemplateListAsset()
        {
            AssetFactory.CreateAssetInProjectWindow<SceneTemplateList>("", "New SceneTemplateList.asset");
        }

        public string ListName = "New Template List";
        public SceneWindowTemplate[] Templates;
    }

    [System.Serializable]
    public struct SceneWindowTemplate
    {
        public string Name;
        [Multiline]
        public string Description;
        [NonNullCheck]
        public SceneAsset Scene;
        public Texture2D ScreenShot;
    }
}
