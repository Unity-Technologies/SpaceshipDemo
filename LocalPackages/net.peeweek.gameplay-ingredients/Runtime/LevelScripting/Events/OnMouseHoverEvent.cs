using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [RequireComponent(typeof(Collider))]
    public class OnMouseHoverEvent : EventBase
    {
        [ReorderableList]
        public Callable[] OnHoverIn;
        [ReorderableList]
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


