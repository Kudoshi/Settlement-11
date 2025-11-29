using Kudoshi.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : Singleton<PlayerCamera>
{
    // Handles only the camera movement. It also turns the player's rotation

    [SerializeField] private Transform m_PlayerTrans;
    [SerializeField] private Transform m_CameraTrans;
    [SerializeField] private float m_Sensitivity;

    private bool m_AllowCamControl;
    
    private Player m_Player;
    private Vector3 m_CamRegularRotation;


    public bool AllowCamControl { get => m_AllowCamControl; }
    public Transform CameraTrans { get => m_CameraTrans; }

    private void Awake()
    {
        m_Player = GetComponent<Player>();
        m_AllowCamControl = true;
    }

    private void Start()
    {
        SetCursorLock();
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (m_AllowCamControl)
        {
            mouseX *= m_Sensitivity;
            mouseY *= m_Sensitivity;
        }

        // Horizontal movement
        this.m_PlayerTrans.Rotate(Vector3.up * mouseX);

        // Vertical movement

        this.m_CamRegularRotation.x -= mouseY;
        this.m_CamRegularRotation.x = Mathf.Clamp(this.m_CamRegularRotation.x, -90f, 90f);
        m_CameraTrans.localRotation = Quaternion.Euler(m_CamRegularRotation);
    }
    
    /// <summary>
    /// Used by PlayerMovement to turn off camera control during dash control
    /// </summary>
    /// <param name="turnOnOff"></param>
    public void SetCameraControl(bool turnOnOff)
    {
        m_AllowCamControl = turnOnOff;
    }

    public void SetCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
