using UnityEngine;

namespace GameplayIngredients.Actions
{
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
    }
}

