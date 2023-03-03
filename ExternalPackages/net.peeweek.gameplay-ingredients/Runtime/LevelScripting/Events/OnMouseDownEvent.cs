using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Mouse Down Event")]
    [RequireComponent(typeof(Collider))]
    public class OnMouseDownEvent : EventBase
    {
        public Callable[] MouseDown;

        private void OnMouseDown()
        {
            Callable.Call(MouseDown, this.gameObject);
        }
    }
}


