using UnityEngine;

public class SwordVFXTest : MonoBehaviour
{
    [SerializeField] private GameObject swordObject;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private PlayerCameraAnimator cameraAnimator;
    [SerializeField] private AnimationClip swingClip1;
    [SerializeField] private AnimationClip swingClip2;
    [SerializeField] private ParticleSystem slashParticle1;
    [SerializeField] private ParticleSystem slashParticle2;
    [SerializeField] private float comboWindowTime = 1f;

    [Header("Position Offset")]
    public float offsetX = 0f;
    public float offsetY = 0f;
    public float offsetZ = 0f;

    [Header("Idle Sway")]
    [SerializeField] private float idleSwaySpeed = 1.5f;
    [SerializeField] private float idleRotationAmount = 2f;

    [Header("Camera Sway")]
    [SerializeField] private float cameraSwayAmount = 50f;
    [SerializeField] private float cameraSwaySmooth = 10f;

    [Header("Movement Sway")]
    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float sprintBobSpeed = 14f;
    [SerializeField] private float bobAmountX = 0.03f;
    [SerializeField] private float bobAmountY = 0.05f;
    [SerializeField] private float bobAmountZ = 0.02f;
    [SerializeField] private float bobSmoothSpeed = 8f;
    [SerializeField] private float walkTiltAmount = 5f;
    [SerializeField] private float sprintTiltAmount = 8f;
    [SerializeField] private float minSpeedForSway = 0.5f;

    [Header("Camera Tilt Match")]
    [SerializeField] private float cameraTiltInfluence = 0.3f;

    [Header("Crouch/Slide")]
    [SerializeField] private float crouchRotationX = 45f;
    [SerializeField] private float crouchRotationY = 0f;
    [SerializeField] private float crouchRotationZ = 0f;
    [SerializeField] private float crouchTransitionSpeed = 8f;


    private Transform swordTransform;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private PlayerMovement playerMovement;
    private Rigidbody playerRb;
    private float movementTimer;
    private Vector2 currentSway;
    private Vector2 targetSway;
    private Quaternion currentCrouchRotation;
    private Vector3 currentBob;
    private Vector3 targetBob;
    private Animation animComponent;
    private bool isSwinging = false;
    private float swingTimer = 0f;
    private bool canCombo = false;
    private float comboTimer = 0f;
    private bool lastWasFirstSwing = true;
    private GameObject playerObject;

    private void Start()
    {
        if (swordObject != null)
        {
            swordTransform = swordObject.transform;
            originalRotation = swordTransform.localRotation;
            originalScale = swordTransform.localScale;
        }

        playerObject = PlayerController.Instance.gameObject;
        playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerRb = playerObject.GetComponent<Rigidbody>();

        if (swordObject != null)
        {
            animComponent = swordObject.GetComponent<Animation>();
            if (animComponent == null)
            {
                animComponent = swordObject.AddComponent<Animation>();
            }
            animComponent.playAutomatically = false;
            if (swingClip1 != null)
            {
                swingClip1.legacy = true;
                animComponent.AddClip(swingClip1, "swing1");
            }
            if (swingClip2 != null)
            {
                swingClip2.legacy = true;
                animComponent.AddClip(swingClip2, "swing2");
            }
        }

        currentCrouchRotation = Quaternion.identity;
    }

    private void LateUpdate()
    {
        if (swordTransform == null || playerCamera == null) return;

        HandleAttackInput();

        if (isSwinging) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        targetSway = new Vector2(-mouseX, -mouseY);
        currentSway = Vector2.Lerp(currentSway, targetSway, Time.deltaTime * cameraSwaySmooth);

        bool isMoving = false;
        bool isSprinting = false;
        bool isCrouching = false;

        if (playerRb != null && playerMovement != null)
        {
            float speed = new Vector2(playerRb.linearVelocity.x, playerRb.linearVelocity.z).magnitude;
            isMoving = speed > minSpeedForSway && playerMovement.grounded;
            isSprinting = playerMovement.isSprinting && isMoving;
            isCrouching = playerMovement.isSliding || (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C));
        }

        if (isMoving)
        {
            float currentBobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
            movementTimer += Time.deltaTime * currentBobSpeed;

        }

        Quaternion targetCrouchRotation = isCrouching
            ? Quaternion.Euler(crouchRotationX, crouchRotationY, crouchRotationZ)
            : Quaternion.identity;
        currentCrouchRotation = Quaternion.Slerp(currentCrouchRotation, targetCrouchRotation, Time.deltaTime * crouchTransitionSpeed);

        float idleTilt = Mathf.Sin(Time.time * idleSwaySpeed) * idleRotationAmount;
        float cameraTiltX = currentSway.y * cameraSwayAmount;
        float cameraTiltZ = currentSway.x * cameraSwayAmount;

        float cameraTiltMatch = 0f;
        if (cameraAnimator != null)
        {
            Vector3 cameraEuler = playerCamera.localRotation.eulerAngles;
            float tiltZ = cameraEuler.z;
            if (tiltZ > 180f) tiltZ -= 360f;
            cameraTiltMatch = tiltZ * cameraTiltInfluence;
        }

        float currentTiltAmount = isSprinting ? sprintTiltAmount : walkTiltAmount;
        float movementTiltX = isMoving ? Mathf.Cos(movementTimer * 0.5f) * currentTiltAmount * 0.5f : 0f;
        float movementTiltZ = isMoving ? Mathf.Sin(movementTimer) * currentTiltAmount : 0f;

        Quaternion finalRotation = originalRotation
            * currentCrouchRotation
            * Quaternion.Euler(0f, 0f, idleTilt)
            * Quaternion.Euler(cameraTiltX, 0f, cameraTiltZ)
            * Quaternion.Euler(movementTiltX, 0f, movementTiltZ)
            * Quaternion.Euler(0f, 0f, cameraTiltMatch);

        targetBob.x = isMoving ? Mathf.Sin(movementTimer) * bobAmountX : 0f;
        targetBob.y = isMoving ? Mathf.Abs(Mathf.Sin(movementTimer * 2f)) * bobAmountY : 0f;
        targetBob.z = isMoving ? Mathf.Cos(movementTimer) * bobAmountZ : 0f;

        currentBob = Vector3.Lerp(currentBob, targetBob, Time.deltaTime * bobSmoothSpeed);

        Vector3 offset = new Vector3(offsetX + currentBob.x, offsetY + currentBob.y, offsetZ + currentBob.z);

        swordTransform.position = playerCamera.position + playerCamera.TransformDirection(offset);
        swordTransform.rotation = playerCamera.rotation * finalRotation;

        Vector3 parentScale = playerObject != null ? playerObject.transform.localScale : Vector3.one;
        swordTransform.localScale = new Vector3(
            originalScale.x / parentScale.x,
            originalScale.y / parentScale.y,
            originalScale.z / parentScale.z
        );
    }

    private void HandleAttackInput()
    {
        if (animComponent == null) return;

        if (isSwinging)
        {
            swingTimer -= Time.deltaTime;
            if (swingTimer <= 0f)
            {
                isSwinging = false;
                canCombo = true;
                comboTimer = comboWindowTime;
            }
        }

        if (canCombo)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                canCombo = false;
            }
        }

        if (Input.GetMouseButtonDown(0) && !isSwinging)
        {
            animComponent.Stop();

            if (canCombo && lastWasFirstSwing && swingClip2 != null)
            {
                SoundManager.Instance.PlaySound("ssfx_sword_shing_slash");
                animComponent.Play("swing2");
                swingTimer = swingClip2.length;
                isSwinging = true;
                canCombo = false;
                lastWasFirstSwing = false;
                if (slashParticle2 != null)
                {
                    slashParticle2.Play();
                }
            }
            else if (swingClip1 != null)
            {
                SoundManager.Instance.PlaySound("sfx_sword_shing_slash");
                animComponent.Play("swing1");
                swingTimer = swingClip1.length;
                isSwinging = true;
                canCombo = false;
                lastWasFirstSwing = true;
                if (slashParticle1 != null)
                {
                    slashParticle1.Play();
                }
            }
        }
    }
}
