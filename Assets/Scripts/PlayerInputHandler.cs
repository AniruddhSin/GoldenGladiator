using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public InputSystem_Actions actions;
    public bool inputAllowed = true;
    public bool isSprinting {get; private set;} = false;
    public bool jumped {get; private set;} = false;
    public float moveX {get; private set;} = 0f;
    [SerializeField] private PauseMenu pauseMenu;
    void Awake()
    {
        actions = new InputSystem_Actions();
    }
    void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += Move;
        actions.Player.Jump.performed += Jump;
        actions.Player.Pause.performed += Pause;

        actions.Player.Move.canceled += Move;
        actions.Player.Jump.canceled += Jump;
        actions.Player.Pause.canceled += Pause;
    }
    void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.Move.performed -= Move;
        actions.Player.Jump.performed -= Jump;
        actions.Player.Pause.performed -= Pause;

        actions.Player.Move.canceled -= Move;
        actions.Player.Jump.canceled -= Jump;
        actions.Player.Pause.canceled -= Pause;
    }


    void Move(InputAction.CallbackContext ctx)
    {
        if (!inputAllowed)
        {
            moveX = 0f;
            isSprinting = false;
            return;
        }
        if (ctx.performed)
        {
            isSprinting = true;
        }else if (ctx.canceled)
        {
            isSprinting = false;
        }
        moveX = ctx.ReadValue<Vector2>().x;
    }

    void Jump(InputAction.CallbackContext ctx)
    {
        if (!inputAllowed)
        {
            jumped = false;
            return;
        }
        if (ctx.performed)
        {
            jumped = true;
        }else if (ctx.canceled)
        {
            jumped = false;
        }
    }

    void Pause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            inputAllowed = !inputAllowed;
            pauseMenu.TogglePause();
        }
    }
    public void enableInput()
    {
        inputAllowed = true;
    }

}
