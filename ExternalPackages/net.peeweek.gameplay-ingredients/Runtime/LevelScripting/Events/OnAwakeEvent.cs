using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Awake Event")]
    public class OnAwakeEvent : EventBase
    {
        public Callable[] onAwake;

        private void Awake()
        {
            Callable.Call(onAwake, gameObject);
        }
    }
}


