using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Destroy Event")]
    public class OnDestroyEvent : EventBase
    {
        public Callable[] onDestroy;

        private void OnDestroy()
        {
            Callable.Call(onDestroy, gameObject);
        }
        
    }
}


