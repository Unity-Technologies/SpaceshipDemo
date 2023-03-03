using UnityEngine;

namespace GameplayIngredients.Events
{
    [AddComponentMenu(ComponentMenu.eventsPath + "On Visibility Event")]
    [RequireComponent(typeof(Renderer))]
    public class OnVisibilityEvent : EventBase
    {
        public Callable[] OnVisible;
        public Callable[] OnInvisible;

        private void OnBecameVisible()
        {
            Callable.Call(OnVisible, this.gameObject);
        }

        private void OnBecameInvisible()
        {
            Callable.Call(OnInvisible, this.gameObject);
        }
    }
}


