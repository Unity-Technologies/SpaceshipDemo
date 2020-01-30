using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [RequireComponent(typeof(Collider))]
    public class OnMouseDownEvent : EventBase
    {
        [ReorderableList]
        public Callable[] MouseDown;

        private void OnMouseDown()
        {
            Callable.Call(MouseDown, this.gameObject);
        }
    }
}


