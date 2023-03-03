using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;

namespace BuildFrontend
{
    internal class BuildTemplateAssetFactory
    {
        [MenuItem("Assets/Create/Build Frontend/Build Template", priority = BuildFrontend.CreateAssetMenuPriority)]
        private static void MenuCreatePostProcessingProfile()
        {
            var icon = EditorGUIUtility.FindTexture("BuildTemplate");
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateBuildTemplateAsset>(), "New BuildTemplate.asset", icon, null);
        }

        public static BuildTemplate CreateAssetAtPath(string path)
        {
            BuildTemplate asset = ScriptableObject.CreateInstance<BuildTemplate>();
            asset.name = Path.GetFileName(path);
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }

    internal class DoCreateBuildTemplateAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            BuildTemplate asset = BuildTemplateAssetFactory.CreateAssetAtPath(pathName);
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}
