using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;
    public Transform orientation;

    [Header("Mouse Look")]
    public float sensitivity = 2f;
    public float maxLookAngle = 90f;
    public float smoothing = 0f;

    [Header("FOV")]
    public bool dynamicFOV = true;
    public float normalFOV = 80f;
    public float crouchFOV = 75f;
    public float sprintFOV = 90f;
    public float slideFOV = 100f;
    public float fovSpeed = 8f;

    [Header("Camera Shake")]
    public float shakeIntensity = 0.3f;
    public float shakeDuration = 0.2f;

    private Camera cam;
    private float xRotation;
    private float yRotation;
    private float currentXRotation;
    private float currentYRotation;
    private float targetFOV;
    private float currentFOV;
    private Vector3 shakeOffset;
    private float shakeTimer;
    private PlayerMovement playerMovement;

    private void Start()
    {
        transform.parent = null;

        cam = GetComponent<Camera>();
        currentFOV = normalFOV;
        targetFOV = normalFOV;

        if (cam != null)
            cam.fieldOfView = currentFOV;

        if (playerHead != null)
        {
            Transform playerRoot = playerHead.root;
            if (playerRoot != null)
                playerMovement = playerRoot.GetComponent<PlayerMovement>();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleFOV();
        HandleShake();
    }

    private void LateUpdate()
    {
        if (playerHead != null)
            transform.position = playerHead.position + shakeOffset;
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        if (smoothing > 0)
        {
            currentXRotation = Mathf.Lerp(currentXRotation, xRotation, 1f - smoothing);
            currentYRotation = Mathf.Lerp(currentYRotation, yRotation, 1f - smoothing);
        }
        else
        {
            currentXRotation = xRotation;
            currentYRotation = yRotation;
        }

        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);

        if (orientation != null)
            orientation.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }

    private void HandleFOV()
    {
        if (!dynamicFOV || cam == null) return;

        targetFOV = normalFOV;

        if (playerMovement != null)
        {
            if (playerMovement.isSliding)
                targetFOV = slideFOV;
            else if (playerMovement.isSprinting)
                targetFOV = sprintFOV;
            else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
                targetFOV = crouchFOV;
        }

        currentFOV = Mathf.SmoothStep(currentFOV, targetFOV, Time.deltaTime * fovSpeed);
        cam.fieldOfView = currentFOV;
    }

    private void HandleShake()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            shakeOffset = Random.insideUnitSphere * shakeIntensity * (shakeTimer / shakeDuration);
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    public void Shake(float intensity, float duration)
    {
        shakeIntensity = intensity;
        shakeDuration = duration;
        shakeTimer = duration;
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}
