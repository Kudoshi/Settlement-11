using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;

    [Header("Movement")]
    public float walkSpeed = 7f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 4f;
    public float slideSpeed = 15f;
    public float groundDrag = 6f;
    public float airDrag = 2f;
    public float acceleration = 10f;
    public float airAcceleration = 6f;

    [Header("Jump")]
    public float jumpForce = 20f;
    public float jumpCooldown = 0.25f;
    public float gravity = 25f;
    public float fallGravityMultiplier = 3f;
    public float maxFallSpeed = 50f;

    [Header("Crouch & Slide")]
    public float crouchHeight = 0.5f;
    public float crouchTransitionSpeed = 0.2f;
    public float slideMinSpeed = 8f;
    public float slideDuration = 1f;
    public float slideBoost = 15f;
    public float slideForce = 50f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public bool grounded;

    [HideInInspector] public bool isSliding;
    [HideInInspector] public bool isSprinting;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector2 input;
    private bool jumpPressed;
    private bool sprintPressed;
    private bool crouchPressed;
    private bool canJump = true;
    private float slideTimer;
    private Vector3 slideDirection;
    private float currentSpeed;
    private MovementState state;

    // Variables to track currently playing sound instances by their ID
    private int sprintSoundEntityID = -1;
    private int slideSoundEntityID = -1;

    // NEW: Variable to track sprint input state for sound activation
    private bool wasSprintInputActive = false;
    // ----------------------------------------------------------------------------

    public MovementState State { get => state; }

    public enum MovementState
    {
        Walking,
        Sprinting,
        Crouching,
        Sliding,
        Air
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleStateChange();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleDrag();
        HandleGravity();
    }

    private void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        jumpPressed = Input.GetButton("Jump");
        sprintPressed = Input.GetKey(KeyCode.LeftShift);
        crouchPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

        if (Input.GetButtonDown("Jump") && grounded && canJump)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C)) && grounded)
        {
            if (sprintPressed)
            {
                Debug.Log("start slide");
                StartSlide();
            }
            else
            {
                float speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
                if (speed > slideMinSpeed && input.magnitude > 0.1f)
                    StartSlide();
                else
                    StartCrouch();
            }
        }

        if ((Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C)))
        {
            StopCrouch();

            // If the player manually stops the slide/crouch, stop the slide sound if playing
            if (isSliding && slideSoundEntityID != -1)
            {
                SoundManager.Instance.StopOneShotByEntityID(slideSoundEntityID);
                slideSoundEntityID = -1;
            }

            isSliding = false;
        }


        // --- NEW: Immediate Sprint Sound Control based on input and grounded status ---

        bool shouldBeSprinting = sprintPressed && grounded;

        // Start sound immediately upon key press while grounded (and not sliding)
        if (shouldBeSprinting && !wasSprintInputActive && !isSliding)
        {
            if (sprintSoundEntityID != -1) // Stop previous instance just in case
                SoundManager.Instance.StopOneShotByEntityID(sprintSoundEntityID);

            // START SPRINT SOUND: Play and store the unique entity ID
            sprintSoundEntityID = SoundManager.Instance.PlaySound("sfx_running_concrete");
        }

        // Stop sound if key is released OR if we leave the ground
        else if ((!shouldBeSprinting && wasSprintInputActive) || (isSliding && sprintSoundEntityID != -1))
        {
            if (sprintSoundEntityID != -1)
            {
                SoundManager.Instance.StopOneShotByEntityID(sprintSoundEntityID);
                sprintSoundEntityID = -1;
            }
        }

        wasSprintInputActive = shouldBeSprinting;

        // --- END NEW SPRINT SOUND CONTROL ---

    }

    private void HandleGroundCheck()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void HandleStateChange()
    {
        MovementState previousState = state;
        isSprinting = false;

        // --- Determine New State ---
        if (isSliding)
        {
            state = MovementState.Sliding;
            currentSpeed = slideSpeed;

            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                isSliding = false;
                StopCrouch();
                // Slide sound stop logic is handled below in the transition check
            }
        }
        else if (crouchPressed && grounded)
        {
            state = MovementState.Crouching;
            currentSpeed = crouchSpeed;
        }
        else if (grounded && sprintPressed)
        {
            // The sound START logic has been moved to HandleInput() for instant feedback.
            state = MovementState.Sprinting;
            currentSpeed = sprintSpeed;
            isSprinting = true;
        }
        else if (grounded)
        {
            state = MovementState.Walking;
            currentSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.Air;
        }

        // --- Sound Stop Logic (Only for timer-based slide exit now) ---

        // 1. Sprint Sound Stop logic is now managed in HandleInput()

        // 2. Stop Slide Sound if exiting Sliding state due to timer
        if (previousState == MovementState.Sliding && state != MovementState.Sliding)
        {
            if (slideSoundEntityID != -1)
            {
                SoundManager.Instance.StopOneShotByEntityID(slideSoundEntityID);
                slideSoundEntityID = -1;
            }
        }
    }

    private void HandleMovement()
    {
        if (isSliding)
        {
            rb.AddForce(slideDirection * slideForce, ForceMode.Force);
            return;
        }

        Vector3 moveDir = orientation.forward * input.y + orientation.right * input.x;
        moveDir.Normalize();

        float accel = grounded ? acceleration : airAcceleration;
        rb.AddForce(moveDir * currentSpeed * accel, ForceMode.Force);
    }

    private void HandleDrag()
    {
        if (isSliding)
            rb.linearDamping = 0.5f;
        else
            rb.linearDamping = grounded ? groundDrag : airDrag;
    }

    private void HandleGravity()
    {
        if (!grounded)
        {
            float gravityForce = gravity;

            if (rb.linearVelocity.y < 0)
                gravityForce *= fallGravityMultiplier;

            rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);

            if (rb.linearVelocity.y < -maxFallSpeed)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -maxFallSpeed, rb.linearVelocity.z);
        }
    }

    private void SpeedControl()
    {
        if (isSliding) return;

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void StartSlide()
    {
        // When sliding starts, we stop the sprint sound immediately if it was playing
        if (sprintSoundEntityID != -1)
        {
            SoundManager.Instance.StopOneShotByEntityID(sprintSoundEntityID);
            sprintSoundEntityID = -1;
        }

        // START SLIDE SOUND: Play and store the unique entity ID
        slideSoundEntityID = SoundManager.Instance.PlaySound("sfx_sliding");

        isSliding = true;
        slideTimer = slideDuration;

        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (currentVelocity.magnitude < 0.5f)
            slideDirection = orientation.forward;
        else
            slideDirection = currentVelocity.normalized;

        transform.DOKill();
        transform.DOScaleY(crouchHeight, crouchTransitionSpeed).SetEase(Ease.OutQuad);

        rb.AddForce(slideDirection * slideBoost, ForceMode.Impulse);
    }

    private void StartCrouch()
    {
        transform.DOKill();
        transform.DOScaleY(crouchHeight, crouchTransitionSpeed).SetEase(Ease.OutQuad);
    }

    private void StopCrouch()
    {
        transform.DOKill();
        transform.DOScaleY(1f, crouchTransitionSpeed).SetEase(Ease.OutBack);
    }
}