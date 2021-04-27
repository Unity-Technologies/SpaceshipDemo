using GameplayIngredients;
using GameplayIngredients.Actions;
using UnityEngine;
using TMPro;
using GameOptionsUtility;

public class FPSManagerSetResultsUIAction : ActionBase
{
    public TextMeshProUGUI QualityText;
    public TextMeshProUGUI OverallFPSText;
    public TextMeshProUGUI WorstFPSText;
    public TextMeshProUGUI BestFPSText;
    public TextMeshProUGUI CPUInfo;
    public TextMeshProUGUI GPUInfo;
    public TextMeshProUGUI RAMInfo;

    public override void Execute(GameObject instigator = null)
    {
        Resolution r = Screen.currentResolution;
        SpaceshipOptions o = GameOption.Get<SpaceshipOptions>();
        FPSManager fpsm = Manager.Get<FPSManager>(); 
        QualityText.text = $"{r.width}×{r.height}@{r.refreshRate}Hz ({Screen.fullScreenMode}) {o.screenPercentage}% SP - {QualitySettings.names[QualitySettings.GetQualityLevel()]} Quality";
        OverallFPSText.text = ((int)(1000 / fpsm.results.avgMs)).ToString();
        WorstFPSText.text = ((int)(1000 / fpsm.results.maxMs)).ToString();
        BestFPSText.text = ((int)(1000 / fpsm.results.minMs)).ToString();
        CPUInfo.text = $"{SystemInfo.processorType} ({SystemInfo.processorCount} threads) @ { (SystemInfo.processorFrequency/1000f).ToString("F2")} GHz. ";
        RAMInfo.text = $"System Memory : { SystemInfo.systemMemorySize / 1000}GB";
        GPUInfo.text = $"{ SystemInfo.graphicsDeviceName} ({ SystemInfo.graphicsDeviceType}) { SystemInfo.graphicsMemorySize / 1000}GB VRAM";
    }
}
