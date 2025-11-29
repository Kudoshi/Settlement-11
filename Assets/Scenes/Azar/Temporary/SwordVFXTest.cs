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
    [SerializeField] private float minSpeedForSway = 0.5f;

    private Transform swordTransform;
    private Quaternion originalRotation;
    private PlayerMovement playerMovement;
    private Rigidbody playerRb;
    private float movementTimer;
    private Vector2 currentSway;
    private Vector2 targetSway;

    private void Start()
    {
        if (swordObject != null)
        {
            swordTransform = swordObject.transform;
            originalRotation = swordTransform.localRotation;
        }

        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<PlayerMovement>();
            playerRb = playerObject.GetComponent<Rigidbody>();
        }
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

        if (playerRb != null && playerMovement != null)
        {
            float speed = new Vector2(playerRb.linearVelocity.x, playerRb.linearVelocity.z).magnitude;
            isMoving = speed > minSpeedForSway && playerMovement.grounded;
            isSprinting = playerMovement.isSprinting && isMoving;
        }

        if (isMoving)
        {
            float bobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
            movementTimer += Time.deltaTime * bobSpeed;
        }

        float idleTilt = Mathf.Sin(Time.time * idleSwaySpeed) * idleRotationAmount;
        float cameraTiltX = currentSway.y * cameraSwayAmount;
        float cameraTiltZ = currentSway.x * cameraSwayAmount;
        float movementTiltZ = isMoving ? Mathf.Sin(movementTimer) * movementTiltAmount : 0f;
        float movementTiltX = isMoving ? Mathf.Cos(movementTimer * 0.5f) * movementTiltAmount * 0.5f : 0f;

        Quaternion finalRotation = originalRotation
            * Quaternion.Euler(0f, 0f, idleTilt)
            * Quaternion.Euler(cameraTiltX, 0f, cameraTiltZ)
            * Quaternion.Euler(movementTiltX, 0f, movementTiltZ);

        swordTransform.rotation = playerCamera.rotation * finalRotation;
    }

    private void LateUpdate()
    {
        if (swordTransform == null || playerCamera == null) return;

        Vector3 offset = new Vector3(offsetX, offsetY, offsetZ);
        swordTransform.position = playerCamera.position + playerCamera.TransformDirection(offset);
    }
}
