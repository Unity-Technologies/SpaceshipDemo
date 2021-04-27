using GameplayIngredients;
using GameplayIngredients.Actions;
using UnityEngine;

public class FPSManagerToggleAction : ActionBase
{
    public ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState Toggle;

    public override void Execute(GameObject instigator = null)
    {
        if(Manager.Has<FPSManager>())
        {
            var manager = Manager.Get<FPSManager>();

            switch (Toggle)
            {
                case ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Disable:
                    manager.FPSRoot.SetActive(false);
                    break;
                case ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Enable:
                    manager.FPSRoot.SetActive(true);
                    break;
                case ToggleGameObjectAction.GameObjectToggle.GameObjectToggleState.Toggle:
                    manager.FPSRoot.SetActive(!manager.FPSRoot.activeInHierarchy);
                    break;
                default:
                    break;
            }
        }
    }
}
