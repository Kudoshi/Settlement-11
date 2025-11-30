using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float _rotateLookThreshold;

    [Header("Nav Agent Info")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _navAgentAccelerationSpeed;
    [SerializeField] private float _navAngularSpeed;

    private NavMeshAgent m_Agent;

    public float MovementSpeed { get => _movementSpeed; }

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        m_Agent.speed = _movementSpeed;
        m_Agent.acceleration = _navAgentAccelerationSpeed;
        m_Agent.angularSpeed = _navAngularSpeed;
        if (m_Agent.isOnNavMesh)
            m_Agent.isStopped = true;
    }

    private void Update()
    {
        
    }

    public void DisableMovement(bool movementDisabled)
    {
        Debug.Log("Disable movement!" + movementDisabled);
        if (m_Agent.isOnNavMesh && m_Agent.enabled)
        {
            m_Agent.isStopped = movementDisabled;
            if (movementDisabled)
            {
                m_Agent.velocity = Vector3.zero;
                m_Agent.ResetPath();
            }
            //Debug.Log($"Movement disabled: {movementDisabled}, isStopped: {m_Agent.isStopped}");
        }
    }

    public void PleaseDisableMovement()
    {
        if (m_Agent.isOnNavMesh && m_Agent.enabled)
        {
            m_Agent.isStopped = false;
            m_Agent.velocity = Vector3.zero;
            m_Agent.ResetPath();
            //Debug.Log($"Movement disabled: {movementDisabled}, isStopped: {m_Agent.isStopped}");
        }
    }

    public void MoveTo(Vector3 position)
    {
        if (m_Agent.isOnNavMesh && m_Agent.enabled)
        {
            if (m_Agent.isStopped)
            {
                m_Agent.isStopped = false;
            }
            m_Agent.SetDestination(position);
        }
    }

    public void LookAt(Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;
        // Only look at the horizontal direction (ignore Y-axis difference)
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(targetDirection.x, 0, targetDirection.z), Vector3.up);

        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _navAngularSpeed * Time.deltaTime);

        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        if (angleDifference > _rotateLookThreshold)
        {
            transform.rotation = newRotation;
        }
    }

    public void SetRotationControl(bool updateRotation)
    {
        if (m_Agent.isOnNavMesh)
        {
            m_Agent.updateRotation = updateRotation;
        }
    }

}
