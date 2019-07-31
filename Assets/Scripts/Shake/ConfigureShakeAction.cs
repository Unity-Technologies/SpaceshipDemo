using GameplayIngredients;
using GameplayIngredients.Actions;
using UnityEngine;

public class ConfigureShakeAction : ActionBase
{
    public bool ShakeEnabled = false;
    public float Attenuation = 0.5f;
    public float MinDelay = 0.5f;
    public float MaxDelay = 1.7f;
    public ShakeManager.Settings MinSettings = new ShakeManager.Settings
    {
        Intensity = 0.1f,
        Position = Vector3.one * -32.0f
    };

    public ShakeManager.Settings MaxSettings = new ShakeManager.Settings
    {
        Intensity = 1.0f,
        Position = Vector3.one * 32.0f
    };

    public override void Execute(GameObject instigator = null)
    {
        var manager = Manager.Get<ShakeManager>();
        manager.ShakeEnabled = ShakeEnabled;
        manager.Attenuation = Attenuation;
        manager.MinDelay = MinDelay;
        manager.MaxDelay = MaxDelay;
        manager.MinSettings = MinSettings;
        manager.MaxSettings = MaxSettings;
    }
}
