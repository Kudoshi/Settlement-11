using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class AI_Enemy2 : MonoBehaviour 
{
    public enum EnemyState { Idle, Chase, Attack, Dead }

    [Header("AI Attack")]
    [SerializeField] private float _detectRange;
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _atklookRotationSpeed;
    [SerializeField] private string _attackAnimTrigger;

    [Header("Bullet")]
    [SerializeField] private Bullet _pfBullet;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _idleAfterAttackTime;
    private bool _bulletIdle;
    private bool _canAttack;

    [Header("Component References")]
    [SerializeField] private Animator _animator;
    private Enemy _enemy;
    private EnemyState _currentState = EnemyState.Idle;

    private float _nextAttackTime;
    private bool _isAttacking = false;


    private void Start()
    {
        _enemy = GetComponent<Enemy>();


        if (_enemy.Enemy2 == null) _enemy.Enemy2 = this;

        _nextAttackTime = Time.time;
        ChangeState(EnemyState.Idle);

        //Util.WaitForSeconds(this, () =>
        //{
        //    StartChasingPlayer();
        //}, 2);
    }

    private void Update()
    {
        if (PlayerController.Instance == null) return;

        CheckStateTransitions();
        ExecuteCurrentState();
    }

    public void ATrigger_CastAttack()
    {
        Bullet bullet = Instantiate(_pfBullet, _shootPoint.transform.position, Quaternion.identity);
        bullet.StartBullet(_bulletSpeed, _attackDamage, 0); bullet.transform.position = _shootPoint.position;
        bullet.transform.LookAt(PlayerController.Instance.transform);
        bullet.StartBullet(_bulletSpeed, _enemy.AttackDamage, 0, 3f);
    }

    public void ATrigger_EndAttack()
    {
        Invoke(nameof(BulletStopIdle), _idleAfterAttackTime);
    }

    private void BulletStopIdle()
    {
        _bulletIdle = false;
        _enemy.EnemyMovement.DisableMovement(false);
        _nextAttackTime = Time.time + _nextAttackTime;
    }

    private void CheckStateTransitions()
    {
        float distanceToPlayer = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);

        switch (_currentState)
        {
            case EnemyState.Idle:
                if (distanceToPlayer <= _detectRange)
                {
                    ChangeState(EnemyState.Chase);
                }
                break;
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
        if (Time.time >= _nextAttackTime)
        {
            _canAttack = true;
        }

        if (_canAttack && !_bulletIdle)
        {
            _canAttack = false;

            _enemy.EnemyMovement.DisableMovement(true);
            _animator.SetTrigger(_attackAnimTrigger);
            _bulletIdle = true;
        }

        // Get direction to target
        Vector3 directionToTarget = PlayerController.Instance.transform.position - transform.position;
        // Create rotation that looks at target (only Y axis)
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));

        // Lerp between current and target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _atklookRotationSpeed);
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

    public void StartChasingPlayer()
    {
        ChangeState(EnemyState.Chase);
    }

    private void OnDrawGizmosSelected()
    {
        // Detect Range – Yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRange);

        // Attack Range – Red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

}
