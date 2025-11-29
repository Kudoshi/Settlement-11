using UnityEngine;
using System.Collections;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem;
#endif

public class BasicPlayerMovement : MonoBehaviour
{
    [Header("Basic Movement Settings")]
    public float Speed = 5f;
    public float SprintSpeed = 10f;
    public float Rotate = 150f;
    public float JumpForce = 6f;

    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Sliding Settings")]
    public float SlideForce = 15f;
    public float SlideDeceleration = 1.5f;
    public float SlideDuration = 0.6f; 
    public float originalColliderHeight;
    public float slideColliderHeight = 0.5f;
    public float ColliderTransitionDuration = 0.3f;

    private bool isSliding = false;
    private CapsuleCollider capsuleCollider;
    private Coroutine slideCoroutine; 

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // stop tipping over

        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider != null)
        {
            originalColliderHeight = capsuleCollider.height;
        }
    }

    void Update()
    {
        float rotateMultiplier = Rotate * Time.deltaTime;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isSliding) // kenot jump while sliding
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

        // Slide
        if (Input.GetKeyDown(KeyCode.LeftControl) && IsGrounded() && !isSliding)
        {
            if (slideCoroutine != null) StopCoroutine(slideCoroutine); // Prevent starting multiple
            slideCoroutine = StartCoroutine(Slide());
        }

        if (isSliding && Input.GetKeyUp(KeyCode.LeftControl))
        {
            EndSlide();
        }
    }

    void FixedUpdate()
    {
        if (isSliding)
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); //decceleration before stop sliding
            rb.AddForce(-horizontalVelocity.normalized * SlideDeceleration, ForceMode.Acceleration);
            return;
        }

        float currentSpeed = Speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = SprintSpeed;
        }

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += transform.forward;

        if (Input.GetKey(KeyCode.S))
            move -= transform.forward;

        if (Input.GetKey(KeyCode.D))
            move += transform.right;

        if (Input.GetKey(KeyCode.A))
            move -= transform.right;

        if (move.magnitude > 0.1f)
        {
            rb.MovePosition(rb.position + move.normalized * currentSpeed * Time.fixedDeltaTime);
        }
    }

    bool IsGrounded()
    {
        Vector3 startPoint = transform.position + Vector3.up * 0.01f;

        return Physics.Raycast(startPoint, Vector3.down, groundCheckDistance, groundLayer);
    }

    /// <summary>
    /// Coroutine to handle the entire slide sequence (force application and max timing).
    /// </summary>
    IEnumerator Slide()
    {
        isSliding = true;
        StartCoroutine(ChangeColliderHeight(slideColliderHeight));

        Vector3 slideDirection = transform.forward;
        float currentForwardSpeed = Vector3.Dot(rb.linearVelocity, slideDirection);
        float desiredSpeed = Mathf.Max(currentForwardSpeed, Speed) + SlideForce;
        rb.linearVelocity = slideDirection * desiredSpeed + Vector3.up * rb.linearVelocity.y;

        yield return new WaitForSeconds(SlideDuration);

        EndSlide();
    }

    /// <summary>
    /// Helper method to safely end the slide.
    /// </summary>
    void EndSlide()
    {
        if (!isSliding) return;

        if (!Physics.Raycast(transform.position, Vector3.up, originalColliderHeight, groundLayer))
        {
            StartCoroutine(ChangeColliderHeight(originalColliderHeight));
        }
        else
        {

        }

        isSliding = false;
    }

    /// <summary>
    /// Coroutine to smoothly change the CapsuleCollider height and center.
    /// </summary>
    IEnumerator ChangeColliderHeight(float targetHeight)
    {
        float elapsed = 0f;
        float startHeight = capsuleCollider.height;
        float startCenterY = capsuleCollider.center.y;
        float targetCenterY = targetHeight / 2f;

        WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();

        while (elapsed < ColliderTransitionDuration)
        {

            elapsed += Time.fixedDeltaTime;
            float t = elapsed / ColliderTransitionDuration;

            float smoothT = t * t * (3f - 2f * t);

            float newHeight = Mathf.Lerp(startHeight, targetHeight, smoothT);
            float newCenterY = Mathf.Lerp(startCenterY, targetCenterY, smoothT);

            capsuleCollider.height = newHeight;
            capsuleCollider.center = new Vector3(capsuleCollider.center.x, newCenterY, capsuleCollider.center.z);

            yield return fixedUpdate;
        }

        capsuleCollider.height = targetHeight;
        capsuleCollider.center = new Vector3(capsuleCollider.center.x, targetCenterY, capsuleCollider.center.z);
    }
}