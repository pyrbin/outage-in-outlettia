using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Globals/Input Reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions, GameInput.IMenuActions
{
    public event UnityAction PauseEvent = delegate { };
    public event UnityAction<float> MoveEvent = delegate { };
    public event UnityAction JumpEvent = delegate { };
    public event UnityAction HoldEvent = delegate { };
    public event UnityAction InteractEvent = delegate { };
    public event UnityAction RetractEvent = delegate { };
    public event UnityAction UsePowerEvent = delegate { };

    public event UnityAction SkipEvent = delegate { };

    private GameInput GameInput;

    private void OnEnable()
    {
        if (GameInput == null)
        {
            GameInput = new GameInput();
            GameInput.Gameplay.SetCallbacks(this);
            GameInput.Menu.SetCallbacks(this);
        }

        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void EnableMenuInput()
    {
        GameInput.Menu.Enable();
        GameInput.Gameplay.Disable();
    }

    public void DisableMenuInput()
    {
        GameInput.Menu.Disable();
    }

    public void EnableGameplayInput()
    {
        GameInput.Menu.Disable();
        GameInput.Gameplay.Enable();
    }

    public void DisableAllInput()
    {
        GameInput.Gameplay.Disable();
    }

    public void OnSkip(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            SkipEvent.Invoke();
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PauseEvent.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent.Invoke(context.ReadValue<float>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            JumpEvent.Invoke();
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            HoldEvent.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            InteractEvent.Invoke();
    }

    public void OnRetract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            RetractEvent.Invoke();
    }

    public void OnUsePower(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            UsePowerEvent.Invoke();
    }
}
