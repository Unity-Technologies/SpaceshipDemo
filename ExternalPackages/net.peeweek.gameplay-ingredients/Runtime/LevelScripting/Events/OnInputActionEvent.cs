using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace GameplayIngredients.Events
{
#if !ENABLE_INPUT_SYSTEM
    [WarnDisabledModule("New Input System")]
#endif
    [AddComponentMenu(ComponentMenu.eventsPath + "On Input Action Event (New Input System)")]
    public class OnInputActionEvent : EventBase
    {
#if ENABLE_INPUT_SYSTEM
        [SerializeField]
        InputAction inputAction;

        public Callable[] onButtonDown;

        private void OnEnable()
        {
            InputActionManager.Request(inputAction, InputAction_performed);
        }
        private void OnDisable()
        {
            InputActionManager.Release(inputAction, InputAction_performed);
        }

        private void InputAction_performed(InputAction.CallbackContext obj)
        {
            Callable.Call(onButtonDown, gameObject);
        }
#endif
    }
}


