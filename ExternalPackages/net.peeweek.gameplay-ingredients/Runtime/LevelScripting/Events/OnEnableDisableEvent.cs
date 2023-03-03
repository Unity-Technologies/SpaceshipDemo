using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Enable|Disable Event")]
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Events/ic-event-enable-disable.png")]
    public class OnEnableDisableEvent : EventBase
    {
        public Callable[] OnEnableEvent;
        public Callable[] OnDisableEvent;

        private void OnEnable()
        {
            Callable.Call(OnEnableEvent, gameObject);
        }

        private void OnDisable()
        {
            Callable.Call(OnDisableEvent, gameObject);
        }
    }
}

