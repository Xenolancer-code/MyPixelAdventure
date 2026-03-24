using UnityEngine;
using UnityEngine.InputSystem;

public class InputReceiver : MonoBehaviour
{
    private PlayerMov playerMov;
    private DefaultInputActions  defaultInputActions;
    
    private void Awake()
    {
        playerMov = GetComponent<PlayerMov>();
        defaultInputActions = GetComponent<DefaultInputActions>();
    }

    private void OnEnable()
    {
        defaultInputActions.Enable();
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (playerMov == null) return;
        playerMov.OnJump();
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        if (playerMov == null) return;
        
    }

    private void OnDisable()
    {
        defaultInputActions.Disable();
    }
}
