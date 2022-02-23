using GameplayIngredients;
using GameplayIngredients.Events;
using UnityEngine;
using UnityEngine.VFX.DebugTools;

public class VFXDebugToggleEvent : EventBase
{
    [SerializeField]
    Callable[] onVisible;
    [SerializeField]
    Callable[] onHidden;

    private void OnEnable()
    {
        VFXDebugRuntimeView.instance.onDebugVisibilityChange += Instance_onDebugVisibilityChange;
    }
    private void OnDisable()
    {
        VFXDebugRuntimeView.instance.onDebugVisibilityChange -= Instance_onDebugVisibilityChange;
    }
    private void Instance_onDebugVisibilityChange(bool visible)
    {
        if (visible)
            Callable.Call(onVisible, this.gameObject);
        else
            Callable.Call(onHidden, this.gameObject);
    }


}
