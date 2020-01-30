using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    [RequireComponent(typeof(Renderer))]
    public class OnVisibilityEvent : EventBase
    {
        [ReorderableList]
        public Callable[] OnVisible;
        [ReorderableList]
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


