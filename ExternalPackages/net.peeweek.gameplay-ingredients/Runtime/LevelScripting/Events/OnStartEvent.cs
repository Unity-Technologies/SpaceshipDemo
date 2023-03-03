using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Start Event")]
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Events/ic-event-start.png")]
    public class OnStartEvent : EventBase
    {
        public Callable[] OnStart;

        private void Start()
        {
            Callable.Call(OnStart, gameObject); 
        }
    }
}


