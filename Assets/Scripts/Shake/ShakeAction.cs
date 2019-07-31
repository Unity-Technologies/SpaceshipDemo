using GameplayIngredients;
using GameplayIngredients.Actions;
using UnityEngine;

public class ShakeAction : ActionBase
{
    public bool ForceShake = false;

    public ShakeManager.Settings ShakeSettings = new ShakeManager.Settings
    {
        Intensity = 1.0f,
        Position = Vector3.zero
    };

    public override void Execute(GameObject instigator = null)
    {
        Manager.Get<ShakeManager>().Shake(ShakeSettings, ForceShake);
    }
}
