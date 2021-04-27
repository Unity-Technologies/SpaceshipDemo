using GameplayIngredients;
using GameplayIngredients.Actions;
using UnityEngine;

public class LightMaterialBindingRigAction : ActionBase
{
    [SerializeField, NonNullCheck]
    LightMaterialBindingRig rig;

    public enum Action
    { 
        Reset,
        Play,
        Pause,
        Stop
    }

    public Action action;

    public override void Execute(GameObject instigator = null)
    {
        switch (action)
        {
            default:
            case Action.Reset:
                rig?.ResetAnimation();
                break;
            case Action.Play:
                rig?.Play();
                break;
            case Action.Pause:
                rig?.Pause();
                break;
            case Action.Stop:
                rig?.Stop();
                break;
        }
    }
}
