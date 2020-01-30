using UnityEngine;

namespace GameplayIngredients.Events
{
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


