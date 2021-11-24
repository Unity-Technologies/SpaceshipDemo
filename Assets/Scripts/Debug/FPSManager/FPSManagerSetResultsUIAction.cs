using GameplayIngredients;
using GameplayIngredients.Actions;
using UnityEngine;
using TMPro;
using GameOptionsUtility;

public class FPSManagerSetResultsUIAction : ActionBase
{
    public TextMeshProUGUI QualityText;
    public TextMeshProUGUI OverallFPSText;
    public TextMeshProUGUI OverallMSText;
    public TextMeshProUGUI OverallMSPerMPixText;
    public TextMeshProUGUI WorstFPSText;
    public TextMeshProUGUI WorstMSText;
    public TextMeshProUGUI BestFPSText;
    public TextMeshProUGUI BestMSText;
    public TextMeshProUGUI CPUInfo;
    public TextMeshProUGUI GPUInfo;
    public TextMeshProUGUI RAMInfo;

    public override void Execute(GameObject instigator = null)
    {
        GraphicOption go = GameOption.Get<GraphicOption>();
        SpaceshipOptions o = GameOption.Get<SpaceshipOptions>();
        float p = o.screenPercentage / 100f;
        float mPix = (go.width * p * go.height * p) / 1000000;
        string sp = o.screenPercentage == 100 ? $"Native" : $"{o.screenPercentage}% SP ({o.upsamplingMethod.ToString()})";

        FPSManager fpsm = Manager.Get<FPSManager>(); 
        QualityText.text = $"{go.width}x{go.height}@{go.refreshRate}Hz ({go.fullScreenMode}) {sp} ({mPix.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)} MPix) - {QualitySettings.names[QualitySettings.GetQualityLevel()]} Quality";
        OverallFPSText.text = (1000 / fpsm.results.avgMs).ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        OverallMSText.text = fpsm.results.avgMs.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        OverallMSPerMPixText.text = (fpsm.results.avgMs / mPix).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        WorstFPSText.text = (1000 / fpsm.results.maxMs).ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        WorstMSText.text = fpsm.results.maxMs.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        BestFPSText.text = (1000 / fpsm.results.minMs).ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        BestMSText.text = fpsm.results.minMs.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        CPUInfo.text = $"{SystemInfo.processorType} ({SystemInfo.processorCount} threads) @ { (SystemInfo.processorFrequency/1000f).ToString("F2", System.Globalization.CultureInfo.InvariantCulture)} GHz. ";
        RAMInfo.text = $"System Memory : { SystemInfo.systemMemorySize / 1000}GB";
        GPUInfo.text = $"{ SystemInfo.graphicsDeviceName} ({ SystemInfo.graphicsDeviceType}) { SystemInfo.graphicsMemorySize / 1000}GB VRAM";
    }
}
