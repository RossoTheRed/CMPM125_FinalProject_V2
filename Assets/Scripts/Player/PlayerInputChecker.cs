using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInputChecker : MonoBehaviour
{
    public InputAction moveKeys {get; private set; }
    public InputAction crouchKey { get; private set; }
    public InputAction jumpKey { get; private set; }
    public InputAction sprintKey { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveKeys = InputSystem.actions.FindAction("Move", true);
        crouchKey = InputSystem.actions.FindAction("Crouch", true);
        jumpKey = InputSystem.actions.FindAction("Jump", true);
        sprintKey = InputSystem.actions.FindAction("Sprint", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Debug_CallInputs(bool constantReport)
    {
        if (constantReport == true)
        {
            Debug.Log($"moveKeys: {moveKeys.ReadValue<Vector2>()}");
            Debug.Log($"jumpKey: {jumpKey.IsPressed()}");
            Debug.Log($"crouchKey: {crouchKey.IsPressed()}");
            Debug.Log($"sprintKey: {sprintKey.IsPressed()}");
        }
        else
        {
            if (moveKeys.IsPressed()) { Debug.Log($"moveKeys: {moveKeys.ReadValue<Vector2>()}"); }
            if (jumpKey.IsPressed()) { Debug.Log($"jumpKey: {jumpKey.IsPressed()}"); }
            if (crouchKey.IsPressed()) { Debug.Log($"crouchKey: {crouchKey.IsPressed()}"); }
            if (sprintKey.IsPressed()) { Debug.Log($"sprintKey: {sprintKey.IsPressed()}"); }
        }
    }
}
