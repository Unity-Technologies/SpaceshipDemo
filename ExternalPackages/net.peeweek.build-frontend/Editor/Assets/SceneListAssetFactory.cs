using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;

namespace BuildFrontend
{
    internal class SceneListAssetFactory
    {
        [MenuItem("Assets/Create/Build Frontend/Scene List", priority = BuildFrontend.CreateAssetMenuPriority)]
        private static void MenuCreatePostProcessingProfile()
        {
            var icon = EditorGUIUtility.FindTexture("SceneList");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateSceneListAsset>(), "New SceneList.asset", icon, null);
        }

        public static SceneList CreateAssetAtPath(string path)
        {
            SceneList asset = ScriptableObject.CreateInstance<SceneList>();
            asset.name = Path.GetFileName(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }

    internal class DoCreateSceneListAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            SceneList asset = SceneListAssetFactory.CreateAssetAtPath(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
