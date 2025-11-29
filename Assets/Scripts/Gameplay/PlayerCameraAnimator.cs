using UnityEngine;

public class PlayerCameraAnimator : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;

    [Header("Head Bob")]
    public bool enableHeadBob = true;
    public float idleBobSpeed = 1.5f;
    public float idleBobAmount = 0.02f;
    public float walkBobSpeed = 10f;
    public float walkBobAmount = 0.05f;
    public float sprintBobSpeed = 14f;
    public float sprintBobAmount = 0.08f;

    [Header("Camera Tilt")]
    public bool enableTilt = true;
    public float movementTiltAmount = 2f;
    public float slideTiltAmount = 15f;
    public float tiltSpeed = 5f;

    [Header("Shake")]
    public float landShakeIntensity = 0.5f;
    public float landShakeDuration = 0.2f;

    private Vector3 originalLocalPosition;
    private float bobTimer;
    private float currentTilt;
    private float targetTilt;
    private PlayerMovement playerMovement;
    private bool wasGrounded;
    private Vector3 shakeOffset;
    private float shakeTimer;
    private float shakeDuration;
    private float shakeIntensity;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;

        if (playerHead != null)
        {
            Transform playerRoot = playerHead.root;
            if (playerRoot != null)
                playerMovement = playerRoot.GetComponent<PlayerMovement>();
        }

        wasGrounded = true;
    }

    private void Update()
    {
        HandleHeadBob();
        HandleTilt();
        HandleShake();
        HandleLanding();
        ApplyEffects();
    }

    private void HandleHeadBob()
    {
        if (!enableHeadBob) return;

        float bobSpeed = idleBobSpeed;
        float bobAmount = idleBobAmount;

        if (playerMovement != null)
        {
            Vector3 velocity = playerMovement.GetComponent<Rigidbody>().linearVelocity;
            float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;

            if (horizontalSpeed > 0.1f && playerMovement.grounded)
            {
                if (playerMovement.isSprinting)
                {
                    bobSpeed = sprintBobSpeed;
                    bobAmount = sprintBobAmount;
                }
                else
                {
                    bobSpeed = walkBobSpeed;
                    bobAmount = walkBobAmount;
                }

                bobTimer += Time.deltaTime * bobSpeed;
            }
            else
            {
                bobTimer += Time.deltaTime * idleBobSpeed;
            }
        }
        else
        {
            bobTimer += Time.deltaTime * idleBobSpeed;
        }
    }

    private void HandleTilt()
    {
        if (!enableTilt)
        {
            targetTilt = 0f;
            return;
        }

        targetTilt = 0f;

        if (playerMovement != null)
        {
            if (playerMovement.isSliding)
            {
                targetTilt = slideTiltAmount;
            }
            else if (playerMovement.grounded)
            {
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                targetTilt = -horizontalInput * movementTiltAmount;
            }
        }

        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
    }

    private void HandleShake()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            float shakeProgress = shakeTimer / shakeDuration;
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeIntensity * shakeProgress,
                Random.Range(-1f, 1f) * shakeIntensity * shakeProgress,
                Random.Range(-1f, 1f) * shakeIntensity * shakeProgress * 0.5f
            );
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    private void HandleLanding()
    {
        if (playerMovement == null) return;

        if (!wasGrounded && playerMovement.grounded)
        {
            Shake(landShakeIntensity, landShakeDuration);
        }

        wasGrounded = playerMovement.grounded;
    }

    private void ApplyEffects()
    {
        Vector3 bobOffset = Vector3.zero;

        if (enableHeadBob)
        {
            float bobSpeed = idleBobSpeed;
            float bobAmount = idleBobAmount;

            if (playerMovement != null)
            {
                Vector3 velocity = playerMovement.GetComponent<Rigidbody>().linearVelocity;
                float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;

                if (horizontalSpeed > 0.1f && playerMovement.grounded)
                {
                    if (playerMovement.isSprinting)
                        bobAmount = sprintBobAmount;
                    else
                        bobAmount = walkBobAmount;
                }
            }

            bobOffset.y = Mathf.Sin(bobTimer) * bobAmount;
            bobOffset.x = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f;
        }

        transform.localPosition = originalLocalPosition + bobOffset + shakeOffset;

        if (enableTilt)
        {
            Vector3 currentRotation = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, currentTilt);
        }
    }

    public void Shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }
}
