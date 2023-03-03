using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;

namespace BuildFrontend
{
    internal class BuildProfileAssetFactory
    {
        [MenuItem("Assets/Create/Build Frontend/Build Profile", priority = BuildFrontend.CreateAssetMenuPriority)]
        private static void MenuCreatePostProcessingProfile()
        {
            var icon = EditorGUIUtility.FindTexture("BuildProfile");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateBuildProfileAsset>(), "New BuildProfile.asset", icon, null);
        }

        public static BuildProfile CreateAssetAtPath(string path)
        {
            BuildProfile asset = ScriptableObject.CreateInstance<BuildProfile>();
            asset.name = Path.GetFileName(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }

    internal class DoCreateBuildProfileAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            BuildProfile asset = BuildProfileAssetFactory.CreateAssetAtPath(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
