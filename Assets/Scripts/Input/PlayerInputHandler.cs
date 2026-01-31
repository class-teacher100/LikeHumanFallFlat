using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool GrabLeftHeld { get; private set; }
    public bool GrabRightHeld { get; private set; }

    InputSystem_Actions _actions;

    void OnEnable()
    {
        _actions = new InputSystem_Actions();
        _actions.Player.Enable();

        _actions.Player.Jump.performed += _ => JumpPressed = true;
        _actions.Player.Jump.canceled += _ => JumpPressed = false;

        _actions.Player.GrabLeft.performed += _ => GrabLeftHeld = true;
        _actions.Player.GrabLeft.canceled += _ => GrabLeftHeld = false;

        _actions.Player.GrabRight.performed += _ => GrabRightHeld = true;
        _actions.Player.GrabRight.canceled += _ => GrabRightHeld = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        _actions.Player.Disable();
        _actions.Dispose();
    }

    void Update()
    {
        MoveInput = _actions.Player.Move.ReadValue<Vector2>();
        LookInput = _actions.Player.Look.ReadValue<Vector2>();
    }

    public void ConsumeJump()
    {
        JumpPressed = false;
    }
}
