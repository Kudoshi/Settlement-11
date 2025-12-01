using UnityEngine;

public class RoomPlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject cigarette;
    [SerializeField] private CanvasGroup interactHUD;
    [SerializeField] private GameObject heldPills;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -20f;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Head Bob")]
    [SerializeField] private float idleBobSpeed = 1.5f;
    [SerializeField] private float idleBobAmount = 1f;
    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float walkBobAmount = 3f;
    [SerializeField] private float sprintBobSpeed = 14f;
    [SerializeField] private float sprintBobAmount = 5f;

    [Header("Cigarette Sway")]
    [SerializeField] private float cigaretteSwaySpeed = 8f;
    [SerializeField] private float cigaretteSwayAmount = 0.5f;

    [Header("Interaction")]
    [SerializeField] private float raycastDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float zoomFOV = 40f;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float timeToZoom = 1f;
    [SerializeField] private float zoomSpeed = 5f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private float bobTimer = 0f;
    private Transform cigaretteTransform;
    private Vector3 cigaretteOriginalPos;
    private Quaternion cigaretteOriginalRot;
    private bool canMove = false;
    private float introTimer = 0f;
    private Animator playerAnimator;
    private RoomObjective currentObjective;
    private Transform heldPillsTransform;
    private Vector3 heldPillsOriginalPos;
    private Quaternion heldPillsOriginalRot;
    private Camera cam;
    private float lookAtTimer = 0f;
    private bool isZoomed = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
        }

        playerAnimator = GetComponent<Animator>();

        if (playerCamera != null)
        {
            cam = playerCamera.GetComponent<Camera>();
            if (cam != null)
            {
                cam.fieldOfView = normalFOV;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cigarette != null)
        {
            cigaretteTransform = cigarette.transform;
            cigaretteOriginalPos = cigaretteTransform.localPosition;
            cigaretteOriginalRot = cigaretteTransform.localRotation;
        }

        if (interactHUD != null)
        {
            interactHUD.alpha = 0f;
        }

        if (heldPills != null)
        {
            heldPillsTransform = heldPills.transform;
            heldPillsOriginalPos = heldPillsTransform.localPosition;
            heldPillsOriginalRot = heldPillsTransform.localRotation;
            heldPills.SetActive(false);
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            introTimer += Time.deltaTime;
            if (introTimer >= 2.4f)
            {
                canMove = true;
                if (playerAnimator != null)
                {
                    Destroy(playerAnimator);
                }
            }
            return;
        }

        HandleMovement();
        HandleLook();
        HandleHeadBob();
        HandleInteraction();
        HandleZoom();
        HandleCigaretteSway();
        HandleHeldPillsSway();
    }

    private int walkSoundEntityID =-1;
    private int runSoundEntityID =-1;
    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isMovingAround = moveX != 0 || moveZ != 0;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (isMovingAround && !isSprinting)
        {
            if (walkSoundEntityID == -1)
                walkSoundEntityID = SoundManager.Instance.PlaySound("sfx_walk_concrete");

            if (runSoundEntityID  != -1)
            {
                SoundManager.Instance.StopOneShotByEntityID(runSoundEntityID);
                runSoundEntityID = -1;
            }

            Debug.Log("walking");
        }
        else if (isMovingAround && isSprinting)
        {
            if (walkSoundEntityID != -1)
            {
                SoundManager.Instance.StopOneShotByEntityID(walkSoundEntityID);
                walkSoundEntityID = -1;
            }

            if (runSoundEntityID == -1)
            {
                runSoundEntityID = SoundManager.Instance.PlaySound("sfx_running");
            }
        }
        else
        {
            if (walkSoundEntityID != -1)
            {
                SoundManager.Instance.StopOneShotByEntityID(walkSoundEntityID);
                walkSoundEntityID = -1;
            }
            if (runSoundEntityID != -1)
            {
                SoundManager.Instance.StopOneShotByEntityID(runSoundEntityID);
                runSoundEntityID = -1;
            }
        }

    }

    private void HandleLook()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadBob()
    {
        if (playerCamera == null) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f) && controller.isGrounded;

        if (isMoving)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float currentBobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
            float currentBobAmount = isSprinting ? sprintBobAmount : walkBobAmount;

            bobTimer += Time.deltaTime * currentBobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * currentBobAmount;

            playerCamera.localRotation = Quaternion.Euler(xRotation + bobOffset, 0f, 0f);
        }
        else
        {
            bobTimer += Time.deltaTime * idleBobSpeed;
            float idleBob = Mathf.Sin(bobTimer) * idleBobAmount;

            playerCamera.localRotation = Quaternion.Euler(xRotation + idleBob, 0f, 0f);
        }
    }

    private void HandleCigaretteSway()
    {
        if (cigaretteTransform == null) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f) && controller.isGrounded;

        if (isMoving)
        {
            float swayX = Mathf.Sin(bobTimer * cigaretteSwaySpeed) * cigaretteSwayAmount * 0.001f;
            float swayY = Mathf.Cos(bobTimer * cigaretteSwaySpeed * 2f) * cigaretteSwayAmount * 0.0005f;
            float swayZ = Mathf.Sin(bobTimer * cigaretteSwaySpeed * 0.5f) * cigaretteSwayAmount * 0.0008f;

            Vector3 targetPos = cigaretteOriginalPos + new Vector3(swayX, swayY, swayZ);
            cigaretteTransform.localPosition = Vector3.Lerp(cigaretteTransform.localPosition, targetPos, Time.deltaTime * 8f);
        }
        else
        {
            cigaretteTransform.localPosition = Vector3.Lerp(cigaretteTransform.localPosition, cigaretteOriginalPos, Time.deltaTime * 8f);
        }

        Quaternion targetRotation = cigaretteOriginalRot * Quaternion.Euler(xRotation * 0.15f, 0f, 0f);
        cigaretteTransform.localRotation = Quaternion.Slerp(cigaretteTransform.localRotation, targetRotation, Time.deltaTime * 10f);
    }

    private void HandleInteraction()
    {
        if (playerCamera == null) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        Debug.DrawRay(playerCamera.position, playerCamera.forward * raycastDistance, Color.green);

        RoomObjective newObjective = null;

        if (Physics.Raycast(ray, out hit, raycastDistance, interactionLayer))
        {
            RoomObjective objective = hit.collider.GetComponent<RoomObjective>();
            if (objective == null)
            {
                objective = hit.collider.GetComponentInParent<RoomObjective>();
            }

            if (objective != null && objective.CanInteract())
            {
                newObjective = objective;
                if (interactHUD != null)
                {
                    interactHUD.alpha = Mathf.Lerp(interactHUD.alpha, 1f, Time.deltaTime * 10f);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    objective.Interact(this);
                }
            }
        }

        if (currentObjective != newObjective)
        {
            if (currentObjective != null)
            {
                currentObjective.SetLookingAt(false);
            }
            currentObjective = newObjective;
            if (currentObjective != null)
            {
                currentObjective.SetLookingAt(true);
            }
        }

        if (currentObjective == null && interactHUD != null)
        {
            interactHUD.alpha = Mathf.Lerp(interactHUD.alpha, 0f, Time.deltaTime * 10f);
        }

        if (currentObjective != null)
        {
            lookAtTimer += Time.deltaTime;
        }
        else
        {
            lookAtTimer = 0f;
        }
    }

    private void HandleZoom()
    {
        if (cam == null) return;

        bool shouldZoom = lookAtTimer >= timeToZoom && currentObjective != null;

        if (shouldZoom && !isZoomed)
        {
            isZoomed = true;
        }
        else if (!shouldZoom && isZoomed)
        {
            isZoomed = false;
        }

        float targetFOV = isZoomed ? zoomFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    private void HandleHeldPillsSway()
    {
        if (heldPillsTransform == null || !heldPills.activeSelf) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f) && controller.isGrounded;

        if (isMoving)
        {
            float swayX = Mathf.Sin(bobTimer * 10f) * 0.004f;
            float swayY = Mathf.Cos(bobTimer * 20f) * 0.003f;
            float swayZ = Mathf.Sin(bobTimer * 5f) * 0.003f;

            Vector3 targetPos = heldPillsOriginalPos + new Vector3(swayX, swayY, swayZ);
            heldPillsTransform.localPosition = Vector3.Lerp(heldPillsTransform.localPosition, targetPos, Time.deltaTime * 8f);
        }
        else
        {
            heldPillsTransform.localPosition = Vector3.Lerp(heldPillsTransform.localPosition, heldPillsOriginalPos, Time.deltaTime * 8f);
        }

        Quaternion targetRotation = heldPillsOriginalRot * Quaternion.Euler(xRotation * 0.25f, 0f, 0f);
        heldPillsTransform.localRotation = Quaternion.Slerp(heldPillsTransform.localRotation, targetRotation, Time.deltaTime * 10f);
    }

    public void EnableHeldPills()
    {
        if (heldPills != null)
        {
            heldPills.SetActive(true);
        }
    }
}
