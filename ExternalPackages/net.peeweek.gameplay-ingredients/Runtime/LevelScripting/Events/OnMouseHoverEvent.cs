using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Mouse Hover Event")]
    [RequireComponent(typeof(Collider))]
    public class OnMouseHoverEvent : EventBase
    {
        public Callable[] OnHoverIn;
        public Callable[] OnHoverOut;


        private void OnMouseEnter()
        {
            Callable.Call(OnHoverIn, this.gameObject);
        }

        private void OnMouseExit()
        {
            Callable.Call(OnHoverOut, this.gameObject);
        }
    }
}


