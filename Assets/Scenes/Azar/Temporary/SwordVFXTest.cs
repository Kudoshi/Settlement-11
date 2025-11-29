using UnityEngine;

public class SwordVFXTest : MonoBehaviour
{
    [SerializeField] private Transform swordPrefab;
    [SerializeField] private Vector3 swordOffset = new Vector3(0.5f, -0.3f, 0.5f);
    [SerializeField] private Vector3 swordRotation = new Vector3(0f, 0f, 0f);

    [Header("Idle Sway")]
    [SerializeField] private float idleSwaySpeed = 1.5f;
    [SerializeField] private float idleSwayAmount = 0.02f;
    [SerializeField] private float idleRotationAmount = 2f;

    [Header("Camera Dynamic Sway")]
    [SerializeField] private float cameraSensitivity = 2f;
    [SerializeField] private float cameraSwaySmooth = 5f;
    [SerializeField] private float maxCameraSway = 0.15f;

    [Header("Movement Sway")]
    [SerializeField] private float movementBobSpeed = 10f;
    [SerializeField] private float movementBobAmount = 0.05f;
    [SerializeField] private float movementTiltAmount = 3f;

    private Transform swordTransform;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 cameraSwayVelocity;
    private Vector2 lastMousePosition;
    private float movementTimer;
    private bool isMoving;

    private void Start()
    {
        if (swordPrefab != null)
        {
            swordTransform = Instantiate(swordPrefab, transform);
            swordTransform.localPosition = swordOffset;
            swordTransform.localRotation = Quaternion.Euler(swordRotation);
        }

        lastMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }

    private void Update()
    {
        if (swordTransform == null) return;

        isMoving = Input.GetKey(KeyCode.W);

        Vector3 idleSway = CalculateIdleSway();
        Vector3 cameraSway = CalculateCameraSway();
        Vector3 movementSway = isMoving ? CalculateMovementSway() : Vector3.zero;

        targetPosition = swordOffset + idleSway + cameraSway + movementSway;

        Quaternion idleRotation = CalculateIdleRotation();
        Quaternion cameraRotation = CalculateCameraRotation();
        Quaternion movementRotation = isMoving ? CalculateMovementRotation() : Quaternion.identity;

        targetRotation = Quaternion.Euler(swordRotation) * idleRotation * cameraRotation * movementRotation;

        swordTransform.localPosition = Vector3.Lerp(swordTransform.localPosition, targetPosition, Time.deltaTime * 10f);
        swordTransform.localRotation = Quaternion.Slerp(swordTransform.localRotation, targetRotation, Time.deltaTime * 8f);
    }

    private Vector3 CalculateIdleSway()
    {
        float x = Mathf.Sin(Time.time * idleSwaySpeed) * idleSwayAmount;
        float y = Mathf.Sin(Time.time * idleSwaySpeed * 0.5f) * idleSwayAmount * 0.5f;
        return new Vector3(x, y, 0f);
    }

    private Quaternion CalculateIdleRotation()
    {
        float z = Mathf.Sin(Time.time * idleSwaySpeed) * idleRotationAmount;
        return Quaternion.Euler(0f, 0f, z);
    }

    private Vector3 CalculateCameraSway()
    {
        Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 mouseDelta = (currentMousePosition - lastMousePosition) * cameraSensitivity * Time.deltaTime;
        lastMousePosition = currentMousePosition;

        Vector3 targetSway = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f);
        targetSway = Vector3.ClampMagnitude(targetSway, maxCameraSway);

        cameraSwayVelocity = Vector3.Lerp(cameraSwayVelocity, targetSway, Time.deltaTime * cameraSwaySmooth);

        return cameraSwayVelocity;
    }

    private Quaternion CalculateCameraRotation()
    {
        float tiltX = -cameraSwayVelocity.y * 50f;
        float tiltZ = -cameraSwayVelocity.x * 30f;
        return Quaternion.Euler(tiltX, 0f, tiltZ);
    }

    private Vector3 CalculateMovementSway()
    {
        movementTimer += Time.deltaTime * movementBobSpeed;
        float bobX = Mathf.Sin(movementTimer) * movementBobAmount;
        float bobY = Mathf.Abs(Mathf.Sin(movementTimer * 2f)) * movementBobAmount;
        return new Vector3(bobX, bobY, 0f);
    }

    private Quaternion CalculateMovementRotation()
    {
        float tiltZ = Mathf.Sin(movementTimer) * movementTiltAmount;
        float tiltX = Mathf.Cos(movementTimer * 0.5f) * movementTiltAmount * 0.5f;
        return Quaternion.Euler(tiltX, 0f, tiltZ);
    }
}
