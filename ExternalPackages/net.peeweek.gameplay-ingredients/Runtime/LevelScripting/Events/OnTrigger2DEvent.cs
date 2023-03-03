using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
#if !MODULE_PHYSICS2D
    [WarnDisabledModule("Physics 2D")]
#endif
    [AddComponentMenu(ComponentMenu.eventsPath + "On Trigger 2D Event")]
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Events/ic-event-trigger.png")]
#if MODULE_PHYSICS2D
    [RequireComponent(typeof(Collider2D))]
#endif
    public class OnTrigger2DEvent : EventBase
    {
        public Callable[] onTriggerEnter;
        public Callable[] onTriggerExit;

        public bool OnlyInteractWithTag = true;
        [EnableIf("OnlyInteractWithTag")]
        public string Tag = "Player";

#if MODULE_PHYSICS2D
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (OnlyInteractWithTag && other.tag == Tag )
            {
                Callable.Call(onTriggerEnter, other.gameObject);
            }
            if (!OnlyInteractWithTag)
            {
                Callable.Call(onTriggerEnter, other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (OnlyInteractWithTag && other.tag == Tag )
            {
                Callable.Call(onTriggerExit, other.gameObject);
            }
            if (!OnlyInteractWithTag)
            {
                Callable.Call(onTriggerExit, other.gameObject);
            }
        }
#endif
    }
}

