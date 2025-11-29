using UnityEngine;

public class SwordVFXTest : MonoBehaviour
{
    [SerializeField] private GameObject swordObject;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject playerObject;

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

    [Header("Movement")]
    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float sprintBobSpeed = 14f;
    [SerializeField] private float movementTiltAmount = 3f;
    [SerializeField] private float movementBobAmount = 0.02f;
    [SerializeField] private float minSpeedForSway = 0.5f;

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

    private void Update()
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
            float bobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
            movementTimer += Time.deltaTime * bobSpeed;
        }

        Quaternion targetCrouchRotation = isCrouching
            ? Quaternion.Euler(crouchRotationX, crouchRotationY, crouchRotationZ)
            : Quaternion.identity;
        currentCrouchRotation = Quaternion.Slerp(currentCrouchRotation, targetCrouchRotation, Time.deltaTime * crouchTransitionSpeed);

        float idleTilt = Mathf.Sin(Time.time * idleSwaySpeed) * idleRotationAmount;
        float cameraTiltX = currentSway.y * cameraSwayAmount;
        float cameraTiltZ = currentSway.x * cameraSwayAmount;
        float movementTiltZ = isMoving ? Mathf.Sin(movementTimer) * movementTiltAmount : 0f;
        float movementTiltX = isMoving ? Mathf.Cos(movementTimer * 0.5f) * movementTiltAmount * 0.5f : 0f;

        Quaternion finalRotation = originalRotation
            * currentCrouchRotation
            * Quaternion.Euler(0f, 0f, idleTilt)
            * Quaternion.Euler(cameraTiltX, 0f, cameraTiltZ)
            * Quaternion.Euler(movementTiltX, 0f, movementTiltZ);

        swordTransform.rotation = playerCamera.rotation * finalRotation;
    }

    private void LateUpdate()
    {
        if (swordTransform == null || playerCamera == null) return;

        bool isMoving = false;
        if (playerRb != null && playerMovement != null)
        {
            float speed = new Vector2(playerRb.linearVelocity.x, playerRb.linearVelocity.z).magnitude;
            isMoving = speed > minSpeedForSway && playerMovement.grounded;
        }

        float bobX = isMoving ? Mathf.Sin(movementTimer) * movementBobAmount : 0f;
        float bobY = isMoving ? Mathf.Abs(Mathf.Sin(movementTimer * 2f)) * movementBobAmount : 0f;

        Vector3 offset = new Vector3(offsetX + bobX, offsetY + bobY, offsetZ);
        swordTransform.position = playerCamera.position + playerCamera.TransformDirection(offset);

        Vector3 parentScale = playerObject != null ? playerObject.transform.localScale : Vector3.one;
        swordTransform.localScale = new Vector3(
            originalScale.x / parentScale.x,
            originalScale.y / parentScale.y,
            originalScale.z / parentScale.z
        );
    }
}
