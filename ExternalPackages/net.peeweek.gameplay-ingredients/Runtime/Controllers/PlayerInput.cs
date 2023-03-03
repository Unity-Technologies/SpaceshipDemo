using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace GameplayIngredients.Controllers
{
    public abstract class PlayerInput : GameplayIngredientsBehaviour
    {
        public enum ButtonState
        {
            Released = 0,
            JustPressed = 1,
            Pressed = 2,
            JustReleased = 3
        }

        public abstract Vector2 Look { get; }
        public abstract Vector2 Movement { get; }
        public abstract ButtonState Jump { get; }

        public abstract void UpdateInput();
        
#if ENABLE_INPUT_SYSTEM
        protected static ButtonState GetButtonControlState(ButtonControl bc)
        {
            if(bc == null)
            {
                return ButtonState.Released;
            }

            if (bc.isPressed)
            {
                if(bc.wasPressedThisFrame)
                    return ButtonState.JustPressed;
                else
                    return ButtonState.Pressed;
            }
            else
            {
                if(bc.wasReleasedThisFrame)
                    return ButtonState.JustReleased;
                else
                    return ButtonState.Released;
            }
        }
#endif


#if ENABLE_LEGACY_INPUT_MANAGER
        protected static ButtonState GetButtonState(string Button)
        {         
            if (Input.GetButton(Button))
            {
                if (Input.GetButtonDown(Button))
                    return ButtonState.JustPressed;
                else
                    return ButtonState.Pressed;
            }
            else
            {
                if (Input.GetButtonUp(Button))
                    return ButtonState.JustReleased;
                else
                    return ButtonState.Released;

            }
        }
#endif
    }
}
