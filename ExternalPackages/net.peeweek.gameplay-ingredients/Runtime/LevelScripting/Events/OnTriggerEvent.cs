using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Trigger Event")]
    [AdvancedHierarchyIcon("Packages/net.peeweek.gameplay-ingredients/Icons/Events/ic-event-trigger.png")]
    [RequireComponent(typeof(Collider))]
    public class OnTriggerEvent : EventBase
    {
        public Callable[] onTriggerEnter;
        public Callable[] onTriggerExit;

        public bool OnlyInteractWithTag = true;
        [EnableIf("OnlyInteractWithTag")]
        public string Tag = "Player";

        private void OnTriggerEnter(Collider other)
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

        private void OnTriggerExit(Collider other)
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
    }
}
