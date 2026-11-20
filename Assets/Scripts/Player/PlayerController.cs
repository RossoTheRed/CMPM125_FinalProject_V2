
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
    [SerializeField] MovementState movementState;
    [SerializeField] PlayerStateMachine stateMachine;

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

    [Header("Sprinting", order = 2)]
    [SerializeField] float sprintSpeedMod = 1.5f;
    float sprintStaminaCost = 0.25f;
    private TimeKeeperScript.Countdown sprintCounter;

    [Header("Crouching", order = 2)]
    [SerializeField] float crouchSpeedMod = 0.70f;
    [SerializeField] float crouchHeightMod = 0.7f;
    float BASEHEIGHT;

    [Header("Jumping", order = 2)]
    [SerializeField] float jumpPower = 6;
    [SerializeField] float gravity = -9.81f;
    Vector3 verticalVelocity = new Vector3(0f, 0f, 0f);
    [SerializeField] float weight = 80f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stateMachine.TransitionState(PlayerStateMachine.StateId.Idle);

        tk = TimeKeeperScript.Instance;

        charController = this.GetComponent<CharacterController>();
        BASEHEIGHT = charController.height;

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
            charController.Move(transform.forward * moveSpeed * inputValue[1]);
            charController.Move(transform.right * moveSpeed * inputValue[0]);
        }
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
        // if (crouchKey.IsPressed() && IsCrouching() == false && IsSprinting() == false && isGrounded)
        // {
        //     ChangeMoveState("crouching");
        //     charController.height = crouchHeightMod;
        //     ChangeHeight(transform.localScale.y * crouchHeightMod);
        // } else if (crouchKey.IsPressed() == false && IsCrouching() && isCramped == false)
        // {
        //     ChangeMoveState("normal");
        //     charController.height = 1;
        //     ChangeHeight(BASEHEIGHT);
        // }

        if (crouchKey.WasPressedThisFrame())
        {
            stateMachine.TransitionState(PlayerStateMachine.StateId.Sneak);
            ChangeHeight(stateMachine.height);
            ChangeSpeed(stateMachine.moveSpeed/100);
        }

        if (crouchKey.IsPressed() == false && isCramped == false)
        {
            stateMachine.TransitionState(PlayerStateMachine.StateId.Idle);
            ChangeHeight(stateMachine.height);
            ChangeSpeed(stateMachine.moveSpeed/100);
        }
    }

    void Sprint()
    { // Add toggleable sprint later too
        // if (sprintKey.IsPressed() && IsSprinting() == false)
        // {
        //     ChangeHeight(BASEHEIGHT);
        //     ChangeMoveState("sprinting");
        // } else if (sprintKey.IsPressed() == false && IsSprinting())
        // {
        //     ChangeMoveState("normal");
        // }

        if (sprintKey.WasPressedThisFrame())
        {
            stateMachine.TransitionState(PlayerStateMachine.StateId.Sprint);
            ChangeHeight(stateMachine.height);
            ChangeSpeed(stateMachine.moveSpeed / 100);
        }

        if (sprintKey.WasReleasedThisFrame())
        {
            stateMachine.TransitionState(PlayerStateMachine.StateId.Idle);
            ChangeHeight(stateMachine.height);
            ChangeSpeed(stateMachine.moveSpeed / 100);
        }
    }

    public void ChangeHeight(float newHeight)
    {
        transform.localScale = new Vector3(transform.localScale.x, newHeight * 2, transform.localScale.z); // Placeholder until we have a player with animations
        charController.height = newHeight;

        ceilingChecker.size = new Vector3(1, 0.1f + (BASEHEIGHT - newHeight), 1);
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
