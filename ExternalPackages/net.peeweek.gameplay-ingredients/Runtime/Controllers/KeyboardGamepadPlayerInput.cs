using System;
using UnityEngine;

namespace GameplayIngredients.Controllers
{
    [Obsolete("KeyboardGamepadPlayerInput is obsolete and will be removed in a later version. Please Use SimplePlayerInput instead.")]
    [AddComponentMenu(ComponentMenu.controllerPath + "Keyboard | Gamepad Player Input (Obsolete)")]
    public class KeyboardGamepadPlayerInput : PlayerInput
    {
        public bool useKeyboardAndMouse { get; set; } = true;
        public bool useGamepad { get; set; } = true;

        [Header("Behaviour")]
        public float LookExponent = 2.0f;
        [Range(0.0f, 0.7f)]
        public float MovementDeadZone = 0.15f;
        [Range(0.0f, 0.7f)]
        public float LookDeadZone = 0.15f;

        [Header("Gamepad Axes")]
        public string MovementHorizontalAxis = "Horizontal";
        public string MovementVerticalAxis = "Vertical";
        public string LookHorizontalAxis = "Look X";
        public string LookVerticalAxis = "Look Y";

        [Header("Mouse Axes")]
        public string MouseHorizontalAxis = "Mouse X";
        public string MouseVerticalAxis = "Mouse Y";

        [Header("Buttons")]
        public string JumpButton = "Jump";

        public override Vector2 Look => m_Look;
        public override Vector2 Movement => m_Movement;

        public override ButtonState Jump => m_Jump;

        Vector2 m_Movement;
        Vector2 m_Look;

        ButtonState m_Jump;

        public override void UpdateInput()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if(useGamepad || useKeyboardAndMouse)
            {
                m_Movement = new Vector2(Input.GetAxisRaw(MovementHorizontalAxis), Input.GetAxisRaw(MovementVerticalAxis));
                if (m_Movement.magnitude < MovementDeadZone)
                    m_Movement = Vector2.zero;
            }

            m_Look = Vector2.zero;
            if(useGamepad)
            {
                Vector2 l = new Vector2(Input.GetAxisRaw(LookHorizontalAxis), Input.GetAxisRaw(LookVerticalAxis));
                Vector2 ln = l.normalized;
                float lm = Mathf.Clamp01(l.magnitude);
                m_Look += ln * Mathf.Pow(Mathf.Clamp01(lm - LookDeadZone) / (1.0f - LookDeadZone), LookExponent);
            }

            if(useKeyboardAndMouse)
                m_Look += new Vector2(Input.GetAxisRaw(MouseHorizontalAxis), Input.GetAxisRaw(MouseVerticalAxis));

            m_Jump = GetButtonState(JumpButton);
#else
            Debug.LogWarning("KeyboardGamepadPlayerInput is only compatible with legacy input");
#endif
        }
    }
}
