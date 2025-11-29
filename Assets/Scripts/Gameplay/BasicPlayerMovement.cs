using UnityEngine;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem;
#endif

public class BasicPlayerMovement : MonoBehaviour
{
    public float Speed = 5f;
    public float SprintSpeed = 10f; 
    public float Rotate = 150f;
    public float JumpForce = 6f;

    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // stop tipping over
    }

    void Update()
    {
        float rotateMultiplier = Rotate * Time.deltaTime;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = Speed;

        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            currentSpeed = SprintSpeed;
        }

        float multiplier = currentSpeed * Time.fixedDeltaTime; 
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move += transform.forward;

        if (Input.GetKey(KeyCode.S))
            move -= transform.forward;

        if (Input.GetKey(KeyCode.D))
            move += transform.right;

        if (Input.GetKey(KeyCode.A))
            move -= transform.right;

        rb.MovePosition(rb.position + move.normalized * currentSpeed * Time.fixedDeltaTime);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}