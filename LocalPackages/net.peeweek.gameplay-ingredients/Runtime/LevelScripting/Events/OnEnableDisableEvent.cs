using NaughtyAttributes;
using UnityEngine.Events;

namespace GameplayIngredients.Events
{
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

