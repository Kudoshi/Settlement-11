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

    [Header("Attack Swings")]
    [SerializeField] private float swingSpeed = 15f;
    [SerializeField] private float swingReturnSpeed = 10f;
    [SerializeField] private float comboWindowTime = 0.8f;
    [SerializeField] private AnimationCurve swingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Swing Pivot Points")]
    [SerializeField] private Vector3 pivotPoint1 = new Vector3(0.2f, 0.3f, 0.2f);
    [SerializeField] private Vector3 pivotPoint2 = new Vector3(-0.2f, 0.3f, 0.2f);
    [SerializeField] private Vector3 pivotPoint3 = new Vector3(0f, 0.4f, 0.1f);
    [SerializeField] private float swingArc1 = 120f;
    [SerializeField] private float swingArc2 = 120f;
    [SerializeField] private float swingArc3 = 140f;
    [SerializeField] private bool showPivotGizmos = true;

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

    private bool isSwinging;
    private float swingTimer;
    private int comboIndex;
    private float comboTimer;
    private Vector3 swingPositionOffset;
    private Quaternion swingRotationOffset;

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
        isSwinging = false;
        comboIndex = 0;
        swingPositionOffset = Vector3.zero;
        swingRotationOffset = Quaternion.identity;
    }

    private void LateUpdate()
    {
        if (swordTransform == null || playerCamera == null) return;

        HandleAttackInput();

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

        UpdateSwingAnimation();

        Vector3 offset = new Vector3(offsetX + currentBob.x + swingPositionOffset.x, offsetY + currentBob.y + swingPositionOffset.y, offsetZ + currentBob.z + swingPositionOffset.z);

        swordTransform.position = playerCamera.position + playerCamera.TransformDirection(offset);
        swordTransform.rotation = playerCamera.rotation * finalRotation * swingRotationOffset;

        Vector3 parentScale = playerObject != null ? playerObject.transform.localScale : Vector3.one;
        swordTransform.localScale = new Vector3(
            originalScale.x / parentScale.x,
            originalScale.y / parentScale.y,
            originalScale.z / parentScale.z
        );
    }

    private void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isSwinging && comboTimer > 0f)
            {
                comboIndex = (comboIndex + 1) % 3;
                comboTimer = comboWindowTime;
            }
            else if (!isSwinging)
            {
                comboIndex = 0;
                comboTimer = comboWindowTime;
            }

            isSwinging = true;
            swingTimer = 0f;
        }

        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
        }
        else if (!isSwinging)
        {
            comboIndex = 0;
        }
    }

    private void UpdateSwingAnimation()
    {
        if (!isSwinging)
        {
            swingPositionOffset = Vector3.Lerp(swingPositionOffset, Vector3.zero, Time.deltaTime * swingReturnSpeed);
            swingRotationOffset = Quaternion.Slerp(swingRotationOffset, Quaternion.identity, Time.deltaTime * swingReturnSpeed);
            return;
        }

        swingTimer += Time.deltaTime * swingSpeed;
        float t = Mathf.Clamp01(swingTimer);
        float curveValue = swingCurve.Evaluate(t);

        Vector3 pivot = Vector3.zero;
        float arcAngle = 0f;
        Vector3 arcAxis = Vector3.forward;

        switch (comboIndex)
        {
            case 0:
                pivot = pivotPoint1;
                arcAngle = swingArc1;
                arcAxis = Vector3.up;
                break;

            case 1:
                pivot = pivotPoint2;
                arcAngle = swingArc2;
                arcAxis = Vector3.up;
                break;

            case 2:
                pivot = pivotPoint3;
                arcAngle = swingArc3;
                arcAxis = Vector3.right;
                break;
        }

        float startAngle = -arcAngle * 0.5f;
        float endAngle = arcAngle * 0.5f;
        float currentAngle = Mathf.Lerp(startAngle, endAngle, curveValue);

        Vector3 direction = (Vector3.zero - pivot).normalized;
        float radius = Vector3.Distance(Vector3.zero, pivot);

        Quaternion rotation = Quaternion.AngleAxis(currentAngle, arcAxis);
        Vector3 rotatedDirection = rotation * direction;

        swingPositionOffset = pivot + rotatedDirection * radius;
        swingRotationOffset = rotation;

        if (t >= 1f)
        {
            isSwinging = false;
            swingTimer = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showPivotGizmos || playerCamera == null) return;

        Gizmos.color = Color.green;
        Vector3 worldPivot1 = playerCamera.position + playerCamera.TransformDirection(pivotPoint1);
        Gizmos.DrawWireSphere(worldPivot1, 0.05f);
        Gizmos.DrawLine(playerCamera.position, worldPivot1);

        Gizmos.color = Color.blue;
        Vector3 worldPivot2 = playerCamera.position + playerCamera.TransformDirection(pivotPoint2);
        Gizmos.DrawWireSphere(worldPivot2, 0.05f);
        Gizmos.DrawLine(playerCamera.position, worldPivot2);

        Gizmos.color = Color.red;
        Vector3 worldPivot3 = playerCamera.position + playerCamera.TransformDirection(pivotPoint3);
        Gizmos.DrawWireSphere(worldPivot3, 0.05f);
        Gizmos.DrawLine(playerCamera.position, worldPivot3);

        DrawArcGizmo(worldPivot1, pivotPoint1, swingArc1, Vector3.up, Color.green, playerCamera);
        DrawArcGizmo(worldPivot2, pivotPoint2, swingArc2, Vector3.up, Color.blue, playerCamera);
        DrawArcGizmo(worldPivot3, pivotPoint3, swingArc3, Vector3.right, Color.red, playerCamera);
    }

    private void DrawArcGizmo(Vector3 worldPivot, Vector3 localPivot, float arcAngle, Vector3 axis, Color color, Transform camera)
    {
        Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);

        Vector3 direction = (Vector3.zero - localPivot).normalized;
        float radius = Vector3.Distance(Vector3.zero, localPivot);

        int segments = 20;
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float angle = Mathf.Lerp(-arcAngle * 0.5f, arcAngle * 0.5f, t);

            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Vector3 rotatedDir = rotation * direction;
            Vector3 localPoint = localPivot + rotatedDir * radius;
            Vector3 worldPoint = camera.position + camera.TransformDirection(localPoint);

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, worldPoint);
            }

            prevPoint = worldPoint;
        }
    }
}
