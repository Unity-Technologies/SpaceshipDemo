using NaughtyAttributes;
using UnityEngine.Events;

namespace GameplayIngredients.Events
{
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Events/ic-event-enable-disable.png")]
    public class OnEnableDisableEvent : EventBase
    {
        [ReorderableList]
        public Callable[] OnEnableEvent;
        [ReorderableList]
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

