using UnityEngine;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    private PlayerMov playerMov;
    private DefaultInputActions defaultInputActions;
    
    private void Awake()
    {
        playerMov = GetComponent<PlayerMov>();
        defaultInputActions = new DefaultInputActions();
    }

    private void OnEnable()
    {
        defaultInputActions.Enable();
        defaultInputActions.Player.Move.performed += OnMove;
        defaultInputActions.Player.Move.canceled += OnMove;
        defaultInputActions.Player.Jump.performed += OnJump;
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (playerMov == null) return;
        playerMov.OnJump();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (playerMov == null) return;
        playerMov.SetMovementVector(ctx.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        defaultInputActions.Player.Move.performed -= OnMove;
        defaultInputActions.Player.Move.canceled -= OnMove;
        defaultInputActions.Player.Move.performed -= OnJump;
        defaultInputActions.Disable();
    }
}
