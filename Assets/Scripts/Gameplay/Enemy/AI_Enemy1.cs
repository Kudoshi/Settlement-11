using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class AI_Enemy1 : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack, Dead }

    [Header("AI Attack")]
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _atklookRotationSpeed;
    [SerializeField] private string _attackAnimTrigger;

    [Header("Component References")]
    [SerializeField] private Animator _animator;
    private Enemy _enemy;
    private EnemyState _currentState = EnemyState.Idle;

    private SphereCollider _detectionCollider;
    private float _detectionRange;

    private float _nextAttackTime;
    private bool _isAttacking = false;

    private void Start()
    {
        _enemy = GetComponent<Enemy>();
        if (_enemy.Enemy1 == null) _enemy.Enemy1 = this;

        _detectionCollider = GetComponent<SphereCollider>();
        if (_detectionCollider != null && _detectionCollider.isTrigger)
        {
            _detectionRange = _detectionCollider.radius;
        }
        else
        {
            Debug.LogError("Missing sphere collider");
        }

        _nextAttackTime = Time.time;
        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (PlayerController.Instance == null) return;

        CheckStateTransitions();
        ExecuteCurrentState();
    }

    private void CheckStateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);

        switch (_currentState)
        {
            case EnemyState.Chase:
                if (distanceToPlayer <= _attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                break;

            case EnemyState.Attack:
                if (distanceToPlayer > _attackRange && !_isAttacking)
                {
                    ChangeState(EnemyState.Chase);
                }
                break;
        }
    }

    private void ExecuteCurrentState()
    {
        switch (_currentState)
        {
            case EnemyState.Chase:
                ChaseBehaviour();
                break;

            case EnemyState.Attack:
                AttackBehaviour();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
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
        if (Time.frameCount % 5 == 0 && PlayerController.Instance != null)
        {
            _enemy.EnemyMovement.MoveTo(PlayerController.Instance.transform.position);
        }
    }

    private void AttackBehaviour()
    {
        _enemy.EnemyMovement.LookAt(PlayerController.Instance.transform);

        if (Time.time >= _nextAttackTime && !_isAttacking)
        {
            _isAttacking = true;
            _enemy.EnemyMovement.DisableMovement(true);
            _animator.SetTrigger(_attackAnimTrigger);
            Debug.Log("Starting attack sequence");
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
                // _animator.SetBool("Move", false);
                break;

            case EnemyState.Chase:
                _enemy.EnemyMovement.DisableMovement(false);
                _enemy.EnemyMovement.SetRotationControl(true);
                _enemy.EnemyMovement.MoveTo(PlayerController.Instance.transform.position);
                _isAttacking = false;
                // _animator.SetBool("Move", true);
                break;

            case EnemyState.Attack:
                _enemy.EnemyMovement.SetRotationControl(false);
                break;

            case EnemyState.Dead:
                _enemy.EnemyMovement.DisableMovement(true);
                // _animator.SetTrigger("Die");
                break;

        }
    }

    public void DealDamage()
    {
        Debug.Log($"Swing hit check. Damage: {_enemy.AttackDamage}");
    }

    public void SetAttackFinished()
    {
        _isAttacking = false;
        _nextAttackTime = Time.time + _attackCooldown;
        _enemy.EnemyMovement.DisableMovement(false);
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);

            if (distanceToPlayer > _attackRange)
            {
                ChangeState(EnemyState.Chase);
            }
        }
        Debug.Log("Attack sequence finished, check for next state.");
    }

}
