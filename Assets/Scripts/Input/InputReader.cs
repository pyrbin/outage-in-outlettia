using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Globals/Input Reader")]
public class InputReader : ScriptableObject, GameInput.IGameplayActions
{
    public event UnityAction PauseEvent = delegate { };

    private GameInput GameInput;

    private void OnEnable()
    {
        if (GameInput == null)
        {
            GameInput = new GameInput();
            GameInput.Gameplay.SetCallbacks(this);
        }

        EnableGameplayInput();
    }

    private void OnDisable()
    {
        DisableAllInput();
    }

    public void EnableGameplayInput()
    {
        GameInput.Gameplay.Enable();
    }

    public void DisableAllInput()
    {
        GameInput.Gameplay.Disable();
    }


    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            PauseEvent.Invoke();
    }
}
