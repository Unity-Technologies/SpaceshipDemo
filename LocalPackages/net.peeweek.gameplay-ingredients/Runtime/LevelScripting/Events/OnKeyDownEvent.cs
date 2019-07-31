using NaughtyAttributes;
using UnityEngine;

namespace GameplayIngredients.Events
{
    public class OnKeyDownEvent : EventBase
    {
        public KeyCode Key = KeyCode.F5;

        [ReorderableList]
        public Callable[] OnKeyDown;

        [ReorderableList]
        public Callable[] OnKeyUp;

        void Update()
        {
            if (Input.GetKeyDown(Key))
                Callable.Call(OnKeyDown, gameObject);

            if (Input.GetKeyUp(Key))
                Callable.Call(OnKeyUp, gameObject);
        }
    }
}


