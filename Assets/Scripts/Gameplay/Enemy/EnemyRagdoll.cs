using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    [SerializeField] private float _strength;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Ragdoll(transform.right, _strength);
        }
    }

    public void Ragdoll(Vector3 direction, float strength)
    {
        Debug.Log("helo");
        _rb.AddForce(strength * direction);

        // Add camera shake when enemy dies
        if (PlayerCamera.Instance != null)
        {
            PlayerCamera.Instance.Shake(0.25f, 0.2f);
        }
    }
}
