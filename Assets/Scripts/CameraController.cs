using UnityEngine;
using UnityEngine.InputSystem;

// Thanks Brackeys
// [link](https://www.youtube.com/watch?v=_QajrabyTJc&list=PLPV2KyIb3jR6Wxj8HaJ_pZhBtaamtXL7J)

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;

    [Range(-90f,90)]
    [SerializeField] float[] rotationLimits = {-70f, 70f};

    [Range(0.01f, 2f)]
    [SerializeField] float verticalSensitivity = 0.5f;
    [Range(0.01f,2f)]
    [SerializeField] float horizontalSensitivity = 0.5f;

    public bool followMouse = true;

    InputAction lookInput;
    InputAction escapeInput;

    float currXRot = 0;
    float currYRot = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lookInput = InputSystem.actions.FindAction("Look", true);
        escapeInput = InputSystem.actions.FindAction("Pause", true);

        LockMouse();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseEscape();
        FollowMouse();
    }

    void TurnHead(Vector3 newRot)
    {
        transform.localEulerAngles = new Vector3(newRot.x,0 , 0);
        player.localEulerAngles = new Vector3(0, newRot.y, 0);
    }

    void FollowMouse()
    {
        if (followMouse == true)
        {
            Vector2 lookValue = lookInput.ReadValue<Vector2>();
            currXRot -= lookValue[1] * verticalSensitivity;
            currYRot += lookValue[0] * horizontalSensitivity;
            currXRot = Mathf.Clamp(currXRot, rotationLimits[0], rotationLimits[1]);

            TurnHead(new Vector3(currXRot, currYRot, 0));
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus == true)
        {
            LockMouse();
        } else
        {
            UnlockMouse();
        }
    }

    void CheckMouseEscape()
    {
        if (escapeInput.WasPressedThisFrame())
        {
            UnlockMouse();
        }
    }

    void LockMouse()
    {
        followMouse = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void UnlockMouse() 
    {
        followMouse = false;
        Cursor.lockState = CursorLockMode.None;
    }
}
