using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInputChecker : MonoBehaviour
{
    InputAction moveKeys;
    InputAction crouchKey;
    InputAction jumpKey;
    InputAction sprintKey;

    public static UnityEvent movePressed {get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveKeys = InputSystem.actions.FindAction("Move", true);
        crouchKey = InputSystem.actions.FindAction("Crouch", true);
        jumpKey = InputSystem.actions.FindAction("Jump", true);
        sprintKey = InputSystem.actions.FindAction("Sprint", true);

        movePressed = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveKeys.WasPressedThisFrame())
        {
            movePressed.Invoke();
        }
    }
}
