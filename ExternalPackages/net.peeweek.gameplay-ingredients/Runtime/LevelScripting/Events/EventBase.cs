using UnityEngine;

namespace GameplayIngredients.Events
{
    [HelpURL(Help.URL + "events-logic-actions")]
    public abstract class EventBase : MonoBehaviour
    {
        protected bool AllowUpdateCalls()
        {
            return GameplayIngredientsSettings.currentSettings.allowUpdateCalls;
        }
        protected bool ForbidUpdateCalls()
        {
            return !GameplayIngredientsSettings.currentSettings.allowUpdateCalls;
        }
    }
}


