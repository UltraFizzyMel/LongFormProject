using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    private CharacterController characterController;

    public float dashSpeed;
    public float dashSpeedChangeFactor;

    public float maxYSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float doubleJumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;
    bool isJumping = false;
    [SerializeField] private int maxJumps = 2;  // 2 = double jump
    private int jumpCount = 0;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    // ====== NEW INPUT SYSTEM ======
    private NewControls playerInput;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool crouchHeld;
    private bool sprintHeld;
    // ==============================

    Vector3 moveDirection;
    Rigidbody rb;

    public int enemyNum = 0;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        dashing,
        air
    }

    public bool dashing;
    public bool sliding;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        // Movement
        playerInput.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerInput.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

        // Jump
        playerInput.Player.Jump.performed += ctx => jumpPressed = true;
        playerInput.Player.Jump.canceled += ctx => jumpPressed = false;

        // Sprint
        playerInput.Player.Sprint.performed += ctx => sprintHeld = true;
        playerInput.Player.Sprint.canceled += ctx => sprintHeld = false;

        // Crouch
        //inputActions.Player.Crouch.performed += ctx => crouchHeld = true;
        //inputActions.Player.Crouch.canceled += ctx => crouchHeld = false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent the Rigidbody from rotating

        startYScale = transform.localScale.y; // Store the initial Y scale of the player
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        Myinput();
        SpeedControl();
        StateHandler();

        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)
        {
            rb.drag = groundDrag; // Apply ground drag when grounded
        }
        else if (!grounded || state == MovementState.dashing || state == MovementState.sliding)
        {
            rb.drag = 0;
        }

        if (grounded && jumpCount > 0)
        {
            jumpCount = 0; // Reset jump count when grounded
            readyToJump = true; // Reset jump readiness
            Invoke(nameof(ResetJump), jumpCooldown); // Reset jump after cooldown
        }
    }

    private bool isGrounded()
    {
        RaycastHit hit;
        float rayLength = 1.2f; // adjust based on player height

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, whatIsGround))
        {
            // Check slope angle
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle <= maxSlopeAngle) // maxSlopeAngle = maybe 45 degrees
                return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ApplyDrag()
    {
        if (grounded && state != MovementState.dashing && state != MovementState.sliding)
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 drag = -flatVel * groundDrag * Time.deltaTime;
            rb.AddForce(drag, ForceMode.VelocityChange);
        }
    }

    private void Myinput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        /*if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Reset jump after cooldown
        }
        else if (Input.GetKeyDown(jumpKey) && isJumping == true)
        {
            readyToJump = false;
            Jump();
        }*/

        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            if (grounded)
            {
                Jump(jumpForce);
            }
            else if (jumpCount < maxJumps)
                Jump(doubleJumpForce);
        }

        else if (Input.GetKeyDown(crouchKey))
        {
            // Crouch
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            //moveSpeed = crouchSpeed; // Set crouch speed
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            // Stop crouching
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            //moveSpeed = walkSpeed; // Reset to walk speed
        }
    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpDashSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boostFactor;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed; // Ensure final value is set
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private MovementState lastState;
    private bool keepMomentum;
    private void StateHandler()
    {
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.angularVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed; // Use sliding speed on slopes
            else
                desiredMoveSpeed = sprintSpeed; // Set sliding speed
        }

        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed; // Set crouch speed
        }

        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        else
        {
            state = MovementState.air;
            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed; // Default to walk speed in air
            else
                desiredMoveSpeed = sprintSpeed; // Air speed can be adjusted
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            // Smoothly lerp the move speed if it has changed significantly
            StopCoroutine(SmoothlyLerpMoveSpeed());
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            // Directly set the move speed if the change is small
            moveSpeed = desiredMoveSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopCoroutine(SmoothlyLerpDashSpeed());
                StartCoroutine(SmoothlyLerpDashSpeed());
            }
            else
            {
                StopCoroutine(SmoothlyLerpDashSpeed());
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed; // Ensure final value is set
    }

    private void MovePlayer()
    {
        if (enemyNum != 0 || state == MovementState.dashing || state == MovementState.sliding) return;

        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            // If on a slope, move in the slope direction
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y < 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); // Prevent sliding down slopes
            }
        }

        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        //rb.useGravity = !OnSlope();
        rb.useGravity = true;

        //moveDirection.Normalize(); // Normalize to ensure consistent speed in all directions
        // Apply the movement to the Rigidbody
        //rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                //Vector3 limitedVel = rb.linearVelocity.normalized * moveSpeed;
                //rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            // Limit the vertical speed
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    private void Jump(float forceToJump)
    {
        jumpCount++;
        readyToJump = true;

        isJumping = true;

        exitingSlope = true; // Set exiting slope to true to handle slope jumping

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity before jumping

        rb.AddForce(transform.up * forceToJump, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        isJumping = false; // Reset jumping state

        readyToJump = true; // Reset the jump cooldown

        exitingSlope = false; // Reset exiting slope state
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle > 0 && angle < maxSlopeAngle;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}