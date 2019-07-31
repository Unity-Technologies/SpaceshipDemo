using GameplayIngredients;
using GameplayIngredients.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class SetResolutionAction : ActionBase
{
    public string ResolutionXSaveName = "Resolution.X";
    public string ResolutionYSaveName = "Resolution.Y";
    public string ResolutionFullScreenSaveName = "Resolution.FullScreen";
    public string ResolutionAASaveName = "Rendering.AntiAliasing";

    public override void Execute(GameObject instigator = null)
    {
        var gsm = Manager.Get<GameSaveManager>();
        int x = gsm.GetInt(ResolutionXSaveName, GameSaveManager.Location.System);
        int y = gsm.GetInt(ResolutionYSaveName, GameSaveManager.Location.System);
        int fs = gsm.GetInt(ResolutionFullScreenSaveName, GameSaveManager.Location.System);
        int aa = gsm.GetInt(ResolutionAASaveName, GameSaveManager.Location.System);

        Screen.SetResolution(x, y, (FullScreenMode)fs);

        var cameraManager = Manager.Get<VirtualCameraManager>();
        var data = cameraManager.GetComponent<HDAdditionalCameraData>();
        data.antialiasing = (HDAdditionalCameraData.AntialiasingMode)aa;
    }
}
