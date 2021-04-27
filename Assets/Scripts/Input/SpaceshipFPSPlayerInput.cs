using GameplayIngredients.Controllers;
using UnityEngine;
using GameOptionsUtility;

public class SpaceshipFPSPlayerInput : PlayerInput
{
    // Public Interface

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

    // Private ~ Properties
    Vector2 m_Movement;
    Vector2 m_Look;

    public override Vector2 Look => m_Look;
    public override Vector2 Movement => m_Movement;
    public override ButtonState Jump => ButtonState.Released;

    SpaceshipOptions options;

    public override void UpdateInput()
    {
        if(options == null)
            options = GameOption.Get<SpaceshipOptions>();

        SpaceshipOptions.FPSKeys keys = options.fpsKeys;

        m_Movement = new Vector2(Input.GetAxisRaw(MovementHorizontalAxis), Input.GetAxisRaw(MovementVerticalAxis));

        if (m_Movement.magnitude < MovementDeadZone)
            m_Movement = Vector2.zero;

        m_Movement.x += (Input.GetKey(keys.left) ? -1 : 0) + (Input.GetKey(keys.right)   ? 1 : 0);
        m_Movement.y += (Input.GetKey(keys.back) ? -1 : 0) + (Input.GetKey(keys.forward) ? 1 : 0);

        float mag = m_Movement.sqrMagnitude;

        if (mag > 1)
            m_Movement.Normalize();

        Vector2 l = new Vector2(Input.GetAxisRaw(LookHorizontalAxis), Input.GetAxisRaw(LookVerticalAxis));
        m_Look = l.normalized * Mathf.Pow(Mathf.Clamp01(Mathf.Clamp01(l.magnitude) - LookDeadZone) / (1.0f - LookDeadZone), LookExponent);
        m_Look += new Vector2(Input.GetAxisRaw(MouseHorizontalAxis), Input.GetAxisRaw(MouseVerticalAxis));
    }
}
