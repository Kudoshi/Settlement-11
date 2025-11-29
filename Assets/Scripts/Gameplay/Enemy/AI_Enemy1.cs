using UnityEngine;
using UnityEngine.XR;

public class AI_Enemy1 : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack, Dead }

    [Header("Component References")]
    private Enemy _enemy;
    private EnemyState _currentState = EnemyState.Idle;

    private SphereCollider _detectionCollider;
    private float _detectionRange;

    private void Start()
    {
        _enemy = GetComponent<Enemy>();

        _detectionCollider = GetComponent<SphereCollider>();
        if (_detectionCollider != null && _detectionCollider.isTrigger)
        {
            _detectionRange = _detectionCollider.radius;
        }
        else
        {
            Debug.LogError("Missing sphere collider");
        }

        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (_currentState == EnemyState.Chase)
        {
            ChaseBehaviour();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            if (_currentState == EnemyState.Idle)
            {
                ChangeState(EnemyState.Chase);
                Debug.Log("Player detected. Entering chase state.");
            }
        }
    }

    private void ChaseBehaviour()
    {
        if (Time.frameCount % 5 == 0 && Player.Instance != null)
        {
            _enemy.EnemyMovement.MoveTo(Player.Instance.transform.position);
        }
    }

    public void ChangeState(EnemyState newState)
    {
        _currentState = newState;
        Debug.Log($"Enemy state changed to: {newState}");

        switch (newState)
        {
            case EnemyState.Idle:
                _enemy.EnemyMovement.DisableMovement(true);
                break;

            case EnemyState.Chase:
                _enemy.EnemyMovement.DisableMovement(false);
                break;
        }
    }

}
