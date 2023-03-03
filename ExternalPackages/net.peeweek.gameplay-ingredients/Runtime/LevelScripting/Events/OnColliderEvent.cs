using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Collider Event")]
    [RequireComponent(typeof(Collider))]
    public class OnColliderEvent : EventBase
    {
        public Callable[] onCollisionEnter;
        public Callable[] onCollisionExit;

        public bool OnlyInteractWithTag = false;
        [EnableIf("OnlyInteractWithTag")]
        public string Tag = "Player";

        private void OnCollisionEnter(Collision other)
        {
            if (OnlyInteractWithTag && other.collider.tag == Tag)
            {
                Callable.Call(onCollisionEnter, other.collider.gameObject);
            }
            if (!OnlyInteractWithTag)
            {
                Callable.Call(onCollisionEnter, other.collider.gameObject);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (OnlyInteractWithTag && other.collider.tag == Tag)
            {
                Callable.Call(onCollisionExit, other.collider.gameObject);
            }
            if (!OnlyInteractWithTag)
            {
                Callable.Call(onCollisionExit, other.collider.gameObject);
            }
        }
    }
}
