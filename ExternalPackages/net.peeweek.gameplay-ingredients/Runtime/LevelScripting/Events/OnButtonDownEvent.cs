using UnityEngine;

namespace GameplayIngredients.Events
{
#if !ENABLE_LEGACY_INPUT_MANAGER
    [WarnDisabledModule("Legacy Input Manager","Player Settings")]
#endif
    [AddComponentMenu(ComponentMenu.eventsPath + "On Button Down Event (Legacy Input)")]
    public class OnButtonDownEvent : EventBase
    {
        public string Button = "Fire1";

        public Callable[] OnButtonDown;
        public Callable[] OnButtonUp;

#if ENABLE_LEGACY_INPUT_MANAGER
        void Update()
        {
            if (Input.GetButtonDown(Button))
                Callable.Call(OnButtonDown, gameObject);

            if (Input.GetButtonUp(Button))
                Callable.Call(OnButtonUp, gameObject);
        }
#endif
    }
}


