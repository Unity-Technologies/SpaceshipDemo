using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class BuildTemplate : BuildFrontendAssetBase
{
    public bool BuildEnabled;

    [Header("Build Template")]
    public string BuildPath;
    public string ExecutableName;
    public bool CleanupBeforeBuild = true;

    public BuildProfile Profile;
    public SceneList SceneList;

    protected override void Awake()
    {
        base.Awake();

        if (BuildPath == null)
            BuildPath = "Build/";
    }

    public BuildReport DoBuild(bool run = false)
    {
        BuildReport report = null;

        if (BuildEnabled)
        {
            try
            {
                EditorUtility.DisplayProgressBar("Build Frontend", $"Building player : {name}", 0.0f);

                BuildOptions options = BuildOptions.None;

                if (Profile.DevPlayer)
                    options |= BuildOptions.Development;

                if(CleanupBeforeBuild && System.IO.Directory.Exists(BuildPath))
                {
                    EditorUtility.DisplayProgressBar("Build Frontend", $"Cleaning up folder : {BuildPath}", 0.05f);
                    System.IO.Directory.Delete(BuildPath, true);
                    System.IO.Directory.CreateDirectory(BuildPath);
                }

                report = BuildPipeline.BuildPlayer(SceneList.scenePaths, BuildPath + ExecutableName, Profile.Target, options);
                if (run)
                {
                    if (
                        report.summary.result == BuildResult.Succeeded ||
                        EditorUtility.DisplayDialog("Run Failed Build", "The build has failed or has been canceled, do you want to attempt to run previous build instead?", "Yes", "No")
                      )
                    {
                        RunBuild();
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e, this);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        else
        {
            Debug.LogWarning("Build is disabled");
        }

        return report;
    }

    public bool canRunBuild
    {
        get
        {
            return System.IO.File.Exists(Application.dataPath + "/../" + BuildPath + ExecutableName);
        }
    }

    public void RunBuild()
    {
        ProcessStartInfo info = new ProcessStartInfo();
        string path = Application.dataPath + "/../" + BuildPath;
        info.FileName = path + ExecutableName;
        info.WorkingDirectory = path;
        info.UseShellExecute = false;

        EditorUtility.DisplayProgressBar("Build Frontend",$"Running Player : {info.FileName}", 1.0f);

        Process process = Process.Start(info);
        
    }
}
