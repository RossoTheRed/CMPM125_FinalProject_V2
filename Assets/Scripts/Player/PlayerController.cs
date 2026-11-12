
using UnityEngine;
using UnityEngine.InputSystem;

// Thanks Brackeys
// [link](https://www.youtube.com/watch?v=_QajrabyTJc&list=PLPV2KyIb3jR6Wxj8HaJ_pZhBtaamtXL7J)


/* To do:
    [Bug] Player stays crouched when falling
    [Bug] Player can jump up steep ramp when they shouldn't be able to
    [Feature] Sprinting should consume stamina
*/

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(StaminaController))]
public class PlayerController : MonoBehaviour
{ 
    public enum MovementState 
    {
        Normal,
        Crouching,
        Sprinting,
    }

    CharacterController charController;
    StaminaController stamController;

    TimeKeeperScript tk;

    InputAction moveKeys;
    InputAction crouchKey;
    InputAction jumpKey;
    InputAction sprintKey;

    [Header("State", order = 1)]
    public MovementState movementState;

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
    [SerializeField] float baseMoveSpeed = 10;

    [Header("Sprinting", order = 2)]
    [SerializeField] float sprintSpeedMod = 1.5f;
    float sprintStaminaCost = 0.25f;
    private TimeKeeperScript.Countdown sprintCounter;

    [Header("Crouching", order = 2)]
    [SerializeField] float crouchSpeedMod = 0.70f;
    [SerializeField] float crouchHeightMod = 0.7f;
    float baseHeight;

    [Header("Jumping", order = 2)]
    [SerializeField] float jumpPower = 6;
    [SerializeField] float gravity = -9.81f;
    Vector3 verticalVelocity = new Vector3(0f, 0f, 0f);
    [SerializeField] float weight = 80f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseHeight = transform.localScale.y;

        charController = this.GetComponent<CharacterController>();
        tk = TimeKeeperScript.Instance;

        stamController = this.GetComponent<StaminaController>();
        sprintCounter = tk.CreateCountdown("sprintCounter", 0.25f, false);

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

        moveKeys = InputSystem.actions.FindAction("Move", true);
        crouchKey = InputSystem.actions.FindAction("Crouch", true);
        jumpKey = InputSystem.actions.FindAction("Jump", true);
        sprintKey = InputSystem.actions.FindAction("Sprint", true);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        CheckHeadBlocked();

        Move();
        Jump();
        Crouch();
        Sprint();
    }

    void Move()
    {
        if (moveKeys.IsPressed())
        {
            Vector2 inputValue = moveKeys.ReadValue<Vector2>(); // [0] Sideways value | [1] Forward value

            float moveSpeed = GetMoveSpeed();

            charController.Move(transform.forward * moveSpeed * inputValue[1]);
            charController.Move(transform.right * moveSpeed * inputValue[0]);
        }
    }

    float GetMoveSpeed()
    {
        float moveSpeed = baseMoveSpeed / 10;
        if (IsSprinting())
        {
            moveSpeed *= sprintSpeedMod;
        }
        else if (IsCrouching())
        {
            moveSpeed *= crouchSpeedMod;
        }

        return(moveSpeed);
    }

    void Jump()
    {
        if (jumpKey.WasPressedThisFrame() && isGrounded && IsCrouching() == false)
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

    void Crouch()
    { //Later add switchable setting for toggle/hold crouch
        // Later make it so crouch deactivates in the air
        if (crouchKey.IsPressed() && IsCrouching() == false && IsSprinting() == false && isGrounded)
        {
            ChangeMoveState("crouching");
            charController.height = crouchHeightMod;
            ChangeHeight(transform.localScale.y * crouchHeightMod);
        } else if (crouchKey.IsPressed() == false && IsCrouching() && isCramped == false)
        {
            ChangeMoveState("normal");
            charController.height = 1;
            ChangeHeight(baseHeight);
        }
    }

    void Sprint()
    { // Add toggleable sprint later too
        if (sprintKey.IsPressed() && IsSprinting() == false && stamController.HaveStamina(sprintStaminaCost))
        {
            ChangeHeight(baseHeight);
            ChangeMoveState("sprinting");
            stamController.DelayRegen();
        } else if (IsSprinting() && (sprintKey.IsPressed() == false || stamController.HaveStamina(sprintStaminaCost) == false))
        {
            ChangeMoveState("normal");
            stamController.ResumeRegen();
        }

        if (IsSprinting())
        {
            if (sprintCounter.IsActive() == false)
            {
                sprintCounter.Reset();
                sprintCounter.Start();

                stamController.Expend(sprintStaminaCost);
            }

            sprintCounter.Count(Time.deltaTime);
        } else if (sprintCounter.IsActive())
        {
            sprintCounter.Pause();
            sprintCounter.Reset();
        }
    }

    void ChangeHeight(float newHeight)
    {
        ceilingChecker.size = new Vector3(1, 0.1f + (baseHeight - newHeight), 1);
    }

    void CheckGrounded()
    {
        isGrounded = groundCheckScript.IsBlocked();
    }

    void CheckHeadBlocked()
    {
        isCramped = ceilingCheckScript.IsBlocked();
    }

    void ChangeMoveState(string newState)
    {
        Debug.Log($"newState: {newState.ToLower()}");

        switch(newState.ToLower())
        {
            case("sprinting"):
                movementState = MovementState.Sprinting;
                break;
            case ("sprint"):
                movementState = MovementState.Sprinting;
                break;

            case ("crouching"):
                movementState = MovementState.Crouching;
                break;
            case ("crouch"):
                movementState = MovementState.Crouching;
                break;

            case ("normal"):
                movementState = MovementState.Normal;
                break;

            default:
                Debug.LogError($"\"{newState}\" is not a valid state for player movement!");
                break;
        }
    }

    bool IsSprinting()
    {
        return(movementState == MovementState.Sprinting);
    }

    bool IsCrouching()
    {
        return(movementState == MovementState.Crouching);
    }

    void Debug_CallInputs(bool constantReport)
    {
        if (constantReport == true)
        {
            Debug.Log($"moveKeys: {moveKeys.ReadValue<Vector2>()}");
            Debug.Log($"jumpKey: {jumpKey.IsPressed()}");
            Debug.Log($"crouchKey: {crouchKey.IsPressed()}");
            Debug.Log($"sprintKey: {sprintKey.IsPressed()}");
        } else
        {
            if (moveKeys.IsPressed()) { Debug.Log($"moveKeys: {moveKeys.ReadValue<Vector2>()}"); }
            if (jumpKey.IsPressed()) { Debug.Log($"jumpKey: {jumpKey.IsPressed()}"); }
            if (crouchKey.IsPressed() ) { Debug.Log($"crouchKey: {crouchKey.IsPressed()}"); }
            if (sprintKey.IsPressed()) { Debug.Log($"sprintKey: {sprintKey.IsPressed()}"); }
        }
    }

    void Debug_LogVelocity()
    {
        // Debug.Log($"Linear Velocity: {myRB.linearVelocity}\nAngular Velocity: {myRB.angularVelocity}");
        Debug.Log($"velocity: {verticalVelocity}");
    }
}
