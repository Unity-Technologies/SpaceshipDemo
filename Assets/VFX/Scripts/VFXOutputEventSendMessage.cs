using GameplayIngredients;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VFXOutputEventSendMessage : VFXOutputEventAbstractHandler
{
    public string message = "VFX-MESSAGE";

    public override bool canExecuteInEditor => false;

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        Messager.Send(message, gameObject);
    }
}
