using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CarAI : MonoBehaviour
{
    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;

    public float wanderRadius = 100f;
    public float wheelRadius = 0.33f;
    
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        SetRandomDestination();
    }

    void Update()
    {
        if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 2f)
        {
            SetRandomDestination();
        }

        UpdateWheels();
    }

    void SetRandomDestination()
    {
        if (!agent.isOnNavMesh) return;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    void UpdateWheels()
    {
        float speed = agent.velocity.magnitude;
        float rotationAmount = (speed / (2 * Mathf.PI * wheelRadius)) * 360f * Time.deltaTime;

        RotateWheel(wheelFL, rotationAmount, true);
        RotateWheel(wheelFR, rotationAmount, true);
        RotateWheel(wheelRL, rotationAmount, false);
        RotateWheel(wheelRR, rotationAmount, false);
    }

    void RotateWheel(Transform wheel, float rotationAmount, bool isFront)
    {
        if (wheel == null) return;

        wheel.Rotate(Vector3.right, rotationAmount);

        if (isFront)
        {
            Vector3 targetDir = agent.steeringTarget - transform.position;
            Vector3 localDir = transform.InverseTransformDirection(targetDir);
            float steeringAngle = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
            
            Vector3 currentEuler = wheel.localEulerAngles;
            wheel.localEulerAngles = new Vector3(currentEuler.x, steeringAngle, currentEuler.z);
        }
    }
}
