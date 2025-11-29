using UnityEngine;

public class SwordVFXTest : MonoBehaviour
{
    [SerializeField] private GameObject swordObject;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerCameraAnimator cameraAnimator;

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

    private void Start()
    {
        if (swordObject != null)
        {
            swordTransform = swordObject.transform;
            originalRotation = swordTransform.localRotation;
            originalScale = swordTransform.localScale;
        }

        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<PlayerMovement>();
            playerRb = playerObject.GetComponent<Rigidbody>();
        }

        currentCrouchRotation = Quaternion.identity;
    }

    private void LateUpdate()
    {
        if (swordTransform == null || playerCamera == null) return;

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
}
