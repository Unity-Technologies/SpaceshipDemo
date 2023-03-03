using UnityEngine;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Application Exit Action")]
    [Callable("Application","Actions/ic-action-exit.png")]
    public class ApplicationExitAction : ActionBase
    {
        public override void Execute(GameObject instigator = null)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
        public override string GetDefaultName()
        {
            return $"Exit Application";
        }
    }
}

