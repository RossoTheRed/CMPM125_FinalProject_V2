
using UnityEngine;
using UnityEngine.InputSystem;

// Thanks Brackeys
// [link](https://www.youtube.com/watch?v=_QajrabyTJc&list=PLPV2KyIb3jR6Wxj8HaJ_pZhBtaamtXL7J)

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    CharacterController myChar;

    InputAction moveKeys;
    InputAction crouchKey;
    InputAction jumpKey;
    InputAction sprintKey;

    [Header("State", order = 1)]
    [SerializeField] bool isCrouching = false;

    [SerializeField] BoxCollider groundChecker;
    ObjectCheck groundCheckScript;
    public bool isGrounded = true;
    [SerializeField] BoxCollider ceilingChecker;
    ObjectCheck ceilingCheckScript;
    public bool isCramped = false;
    
    [SerializeField] bool isSprinting = false;

    [Header("Movement", order = 1)]
    [SerializeField] float baseMoveSpeed = 10;
    [SerializeField] float sprintSpeedMod = 1.5f;

    [Header("Crouching", order=2)]
    [SerializeField] float crouchSpeedMod = 0.70f;
    [SerializeField] float crouchHeightMod = 0.7f;
    float BASEHEIGHT;

    [Header("Jumping", order = 2)]
    [SerializeField] float jumpPower = 6;
    [SerializeField] float gravity = -9.81f;
    Vector3 verticalVelocity = new Vector3(0f, 0f, 0f);
    [SerializeField] float weight = 80f;

    [Header("Debug Tools", order = 1)]
    [SerializeField] bool Debug_logInputs = false;
    [SerializeField] bool Debug_constantCalls = false;
    [SerializeField] bool Debug_logVelocity = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BASEHEIGHT = transform.localScale.y;

        myChar = this.GetComponent<CharacterController>();

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

        Debug_CallInputs(Debug_constantCalls);
        Debug_LogVelocity();
    }
    // void FixedUpdate()
    // {

    // }

    void Move()
    {
        if (moveKeys.IsPressed() == true)
        {
            Vector2 inputValue = moveKeys.ReadValue<Vector2>(); // [0] Sideways value | [1] Forward value

            float moveSpeed = GetMoveSpeed();

            myChar.Move(transform.forward * moveSpeed * inputValue[1]);
            myChar.Move(transform.right * moveSpeed * inputValue[0]);
        }
    }

    float GetMoveSpeed()
    {
        float moveSpeed = baseMoveSpeed / 10;
        if (isSprinting == true)
        {
            moveSpeed *= sprintSpeedMod;
        }
        else if (isCrouching == true)
        {
            moveSpeed *= crouchSpeedMod;
        }

        return(moveSpeed);
    }

    void Jump()
    {
        if (jumpKey.WasPressedThisFrame() && isGrounded == true && isCrouching == false)
        {
            verticalVelocity[1] = Mathf.Sqrt(jumpPower * -2 * gravity);
        }

        if (isCramped && verticalVelocity.y > 0)
        {
            verticalVelocity = Vector3.zero;
        }

        CheckGravity();

        myChar.Move(verticalVelocity * Time.deltaTime);
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
        if (crouchKey.IsPressed() && isCrouching == false && isSprinting == false)
        {
            isCrouching = true;
            myChar.height = crouchHeightMod;
            ChangeHeight(transform.localScale.y * crouchHeightMod);
        } else if (crouchKey.IsPressed() == false && isCrouching == true && isCramped == false)
        {
            isCrouching = false;
            myChar.height = 1;
            ChangeHeight(BASEHEIGHT);
        }
    }

    void ChangeHeight(float newHeight)
    {
        transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);

        ceilingChecker.size = new Vector3(1, 0.1f + (BASEHEIGHT - newHeight), 1);
    }

    void Sprint()
    { // Add toggleable sprint later too
        if (sprintKey.IsPressed() == true && isSprinting == false)
        {
            isCrouching = false;
            ChangeHeight(BASEHEIGHT);
            isSprinting = true;
        } else if (sprintKey.IsPressed() == false && isSprinting == true)
        {
            isSprinting = false;
        }
    }

    void CheckGrounded()
    {
        isGrounded = groundCheckScript.IsBlocked();
    }

    void CheckHeadBlocked()
    {
        isCramped = ceilingCheckScript.IsBlocked();
    }

    void Debug_CallInputs(bool constant)
    {
        if (Debug_logInputs == false) { return; }

        if (constant == true)
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
        if (Debug_logVelocity == false) { return; }

        // Debug.Log($"Linear Velocity: {myRB.linearVelocity}\nAngular Velocity: {myRB.angularVelocity}");
        Debug.Log($"velocity: {verticalVelocity}");
    }
}
