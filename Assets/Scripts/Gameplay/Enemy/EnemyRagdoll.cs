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
    }
}
