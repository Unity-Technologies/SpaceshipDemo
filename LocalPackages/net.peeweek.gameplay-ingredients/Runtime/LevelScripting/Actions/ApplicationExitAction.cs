using UnityEngine;

namespace GameplayIngredients.Actions
{
    public class ApplicationExitAction : ActionBase
    {
        public override void Execute(GameObject instigator = null)
        {
            Application.Quit();
        }
    }
}

