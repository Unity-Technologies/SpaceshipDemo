using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayIngredients.Controllers
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        public bool Paused = false;

        [Header("Objects")]
        [NonNullCheck]
        public Transform m_Camera;
        [NonNullCheck]
        public PlayerInput m_Input;

        [Header("Metrics")]
        [Range(1.0f, 2.5f)]
        public float PlayerHeight = 1.82f;
        [Range(0.35f, 2.0f)]
        public float PlayerRadius = 0.5f;
        public float PlayerWeight = 3.0f;

        [Header("Movement")]
        [Range(0.0f, 12.5f)]
        public float MoveSpeed = 3.0f;
        public float MaximumFallVelocity = 12f;
        public float TurnSpeed = 180;

        [Header("Jump")]
        public bool EnableJump = true;
        public float JumpImpulse = 7.0f;

        [Header("Look")]
        public float PitchSpeed = 180;
        public float MaxPitch = 80;

        private CharacterController m_Controller;
        private float m_Fall = 0.0f;
        private float m_Pitch = 0.0f;
        private bool m_Grounded = false;

        public void Start()
        {
            m_Controller = GetComponent<CharacterController>();
        }

        public void Update()
        {
            if (m_Camera == null) return;
            if (m_Input == null) return;

            if (!Paused)
            {
                m_Input.UpdateInput();
                UpdateRotation();
                UpdatePlayerMovement();
            }
        }

        public void SetPaused(bool paused)
        {
            Paused = paused;
        }

        public void OnValidate()
        {
            if (m_Controller == null)
                m_Controller = GetComponent<CharacterController>();

            float realHeight = PlayerHeight + PlayerRadius;
            m_Controller.center = new Vector3(0, realHeight / 2, 0);
            m_Controller.height = realHeight;
            m_Controller.radius = PlayerRadius;

            m_Camera.transform.localPosition = new Vector3(0, PlayerHeight, 0);
        }

        public void UpdateRotation()
        {
            m_Pitch = Mathf.Clamp(m_Pitch - (Time.deltaTime * PitchSpeed * m_Input.Look.y), -MaxPitch, MaxPitch);
            m_Camera.transform.localEulerAngles = new Vector3(m_Pitch, 0, 0);
            transform.Rotate(transform.up, Time.deltaTime * TurnSpeed * m_Input.Look.x);
        }

        public void UpdatePlayerMovement()
        {
            Vector3 move = (transform.forward * m_Input.Movement.y + transform.right * m_Input.Movement.x) * MoveSpeed;

            m_Fall += PlayerWeight * 9.80665f * Time.deltaTime;
            m_Fall = Mathf.Min(m_Fall, MaximumFallVelocity);
            move += m_Fall * (-transform.up);

            if (m_Grounded)
            {
                if (EnableJump && m_Input.Jump == PlayerInput.ButtonState.JustPressed)
                {
                    m_Fall = -JumpImpulse;
                }
            }

            m_Controller.Move(move * Time.deltaTime);
            m_Grounded = m_Controller.isGrounded;
        }

        public void SetPlayerHeight(float value)
        {
            PlayerHeight = value;
            OnValidate();
        }

        public void SetMoveSpeed(float value)
        {
            MoveSpeed = value;
        }
    }
}
