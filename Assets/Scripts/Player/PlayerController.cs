
using UnityEngine;
using UnityEngine.InputSystem;

// Thanks Brackeys
// [link](https://www.youtube.com/watch?v=_QajrabyTJc&list=PLPV2KyIb3jR6Wxj8HaJ_pZhBtaamtXL7J)


/* To do:

*/

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(StaminaController))]
public class PlayerController : MonoBehaviour
{ 
    [SerializeField] CharacterController charController;
    public PlayerInputChecker inputChecker; // Should be set by the Player's state machine

    [Header("State", order = 1)]

    [SerializeField] BoxCollider groundChecker;
    ObjectCheck groundCheckScript;
    [Tooltip("Is the player on the ground? Read only!")]
    public bool isGrounded = true;
    [Tooltip("The object that checks for overhead obstructions. Gets resized when crouching.")]
    [SerializeField] BoxCollider ceilingChecker;
    ObjectCheck ceilingCheckScript;
    [Tooltip("Is the player blocked from above? Read only!")]
    public bool isCramped = false;

    [Header("Movement", order = 1)]
    [SerializeField] float moveSpeed;

    [Header("Crouching", order = 2)]
    float baseHeight;

    [Header("Jumping", order = 2)]
    [SerializeField] float jumpPower = 6;
    [SerializeField] float gravity = -9.81f;
    Vector3 verticalVelocity = new Vector3(0f, 0f, 0f);
    [SerializeField] float weight = 80f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseHeight = charController.height;

        groundCheckScript = groundChecker.gameObject.GetComponent<ObjectCheck>();
        if (groundCheckScript == null)
        {
            groundChecker.gameObject.AddComponent<ObjectCheck>();
            groundCheckScript = groundChecker.gameObject.GetComponent<ObjectCheck>();
        }

        ceilingCheckScript = ceilingChecker.gameObject.GetComponent<ObjectCheck>();
        if (ceilingCheckScript == null)
        {
            ceilingChecker.gameObject.AddComponent<ObjectCheck>();
            ceilingCheckScript = ceilingChecker.gameObject.GetComponent<ObjectCheck>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckGrounded();
        CheckHeadBlocked();

        Move();
        Jump();
    }

    void Move()
    {
        if (inputChecker.moveKeys.IsPressed())
        {
            Vector2 inputValue = inputChecker.moveKeys.ReadValue<Vector2>(); // [0] Sideways value | [1] Forward value
            charController.Move(transform.forward * moveSpeed * inputValue[1]);
            charController.Move(transform.right * moveSpeed * inputValue[0]);
        }
    }

    void Jump()
    {
        if (inputChecker.jumpKey.WasPressedThisFrame() && isGrounded)
        {
            verticalVelocity[1] = Mathf.Sqrt(jumpPower * -2 * gravity);
        }

        if (isCramped && verticalVelocity.y > 0)
        {
            verticalVelocity = Vector3.zero;
        }

        CheckGravity();

        charController.Move(verticalVelocity * Time.deltaTime);
    }

    void CheckGravity()
    {
        if (isGrounded == false)
        {
            verticalVelocity[1] += gravity * weight * Time.deltaTime;
        } else if (verticalVelocity.y < 0)
        {
            verticalVelocity = Vector3.zero;
        }
    }

    public void ChangeHeight(float newHeight)
    {
        transform.localScale = new Vector3(transform.localScale.x, newHeight * 2, transform.localScale.z); // Placeholder until we have a player with animations
        charController.height = newHeight;

        ceilingChecker.size = new Vector3(1, 0.1f + (baseHeight - newHeight), 1);
    }

    public void ChangeSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    void CheckGrounded()
    {
        isGrounded = groundCheckScript.IsBlocked();
    }

    void CheckHeadBlocked()
    {
        isCramped = ceilingCheckScript.IsBlocked();
    }

    void Debug_LogVelocity()
    {
        // Debug.Log($"Linear Velocity: {myRB.linearVelocity}\nAngular Velocity: {myRB.angularVelocity}");
        Debug.Log($"velocity: {verticalVelocity}");
    }
}
