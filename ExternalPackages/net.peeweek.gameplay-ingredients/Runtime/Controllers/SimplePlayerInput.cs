using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif


namespace GameplayIngredients.Controllers
{
    [AddComponentMenu(ComponentMenu.controllerPath + "Simple Player Input")]
    public class SimplePlayerInput : PlayerInput
    {
#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
        public bool preferLegacyInput;
#endif

#if ENABLE_INPUT_SYSTEM
        [Header("Input System Package")]
        public InputAction move;
        public InputAction look;
        public InputAction jump;
#endif
    
#if ENABLE_LEGACY_INPUT_MANAGER
        [Header("Legacy Input")]
        public string MovementHorizontalAxis = "Horizontal";
        public string MovementVerticalAxis = "Vertical";
        public string LookHorizontalAxis = "Mouse X";
        public string LookVerticalAxis = "Mouse Y";
        public string JumpButton = "Jump";
#endif
        public override Vector2 Look => m_Look;
        public override Vector2 Movement => m_Movement;

        public override ButtonState Jump => m_Jump;

        Vector2 m_Movement;
        Vector2 m_Look;
        ButtonState m_Jump;


        private void Reset()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            InitializeValuesLegacy();
#endif
#if ENABLE_INPUT_SYSTEM
            InitializeValuesNIS();
#endif
        }


#if ENABLE_LEGACY_INPUT_MANAGER
        [ContextMenu("Initialize Values (Legacy Input)")]
        void InitializeValuesLegacy()
        {
            MovementHorizontalAxis = "Horizontal";
            MovementVerticalAxis = "Vertical";
            LookHorizontalAxis = "Mouse X";
            LookVerticalAxis = "Mouse Y";
            JumpButton = "Jump";
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
#endif
#if ENABLE_INPUT_SYSTEM
        [ContextMenu("Initialize Values (Input System Package)")]
        void InitializeValuesNIS()
        {
            move = new InputAction("Left Stick", InputActionType.Value, "<Gamepad>/leftStick");
            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            look = new InputAction("Right Stick", InputActionType.Value, "<Gamepad>/rightStick");
            look.AddBinding("<Pointer>/Delta")
                .WithProcessor("ScaleVector2(x=0.1,y=0.1)");
            jump = new InputAction("Jump", InputActionType.Button, "<Gamepad>/buttonSouth");
            jump.AddBinding("<Keyboard>/space");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
#endif
        public override void UpdateInput()
        {
#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            if(preferLegacyInput)
#endif           
#if ENABLE_LEGACY_INPUT_MANAGER
            {
                m_Movement = new Vector2(Input.GetAxis(MovementHorizontalAxis), Input.GetAxis(MovementVerticalAxis));
                m_Look = new Vector2(Input.GetAxis(LookHorizontalAxis), Input.GetAxis(LookVerticalAxis));
                m_Jump = GetButtonState(JumpButton);
            }
#endif
#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            else
#endif
#if ENABLE_INPUT_SYSTEM
            {
                if (!move.enabled)
                        move.Enable();
                if(!look.enabled)
                        look.Enable();
                if(!jump.enabled)
                        jump.Enable();
                        
                m_Movement = move.ReadValue<Vector2>();
                m_Look = look.ReadValue<Vector2>();
                m_Jump = GetButtonControlState(jump.activeControl as ButtonControl);
            }
#endif

        }
    }
}
