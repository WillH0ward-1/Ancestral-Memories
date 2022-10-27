using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public bool walking;

    public void IsWalking(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            walking = true;
        }
        if (context.performed)
        {
            walking = true;

        }
        if (context.canceled)
        {
            walking = false;
        }
    }
}
