using UnityEngine;
using UnityEngine.UI;

namespace GameplayIngredients.Actions
{
    [AddComponentMenu(ComponentMenu.actionsPath + "Focus UI Action")]
    [Callable("UI", "Actions/ic-action-ui.png")]
    public class FocusUIAction : ActionBase
    {
        public Selectable UIObjectToFocus;

        public override void Execute(GameObject instigator = null)
        {
            Manager.Get<UIEventManager>().FocusUI(UIObjectToFocus);
        }

        public override string GetDefaultName()
        {
            return $"Focus UI : '{UIObjectToFocus?.name}'";
        }
    }
}
