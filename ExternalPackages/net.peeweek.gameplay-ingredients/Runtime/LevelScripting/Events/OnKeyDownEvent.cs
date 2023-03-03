using UnityEngine;

namespace GameplayIngredients.Events
{
#if !ENABLE_LEGACY_INPUT_MANAGER
    [WarnDisabledModule("Legacy Input Manager","Player Settings")]
#endif
    [AddComponentMenu(ComponentMenu.eventsPath + "On Key Down Event (Legacy Input)")]
    public class OnKeyDownEvent : EventBase
    {
        public KeyCode Key = KeyCode.F5;

        public Callable[] OnKeyDown;
        public Callable[] OnKeyUp;

#if ENABLE_LEGACY_INPUT_MANAGER
        void Update()
        {

            if (Input.GetKeyDown(Key))
                Callable.Call(OnKeyDown, gameObject);

            if (Input.GetKeyUp(Key))
                Callable.Call(OnKeyUp, gameObject);
        }
#endif
    }
}


