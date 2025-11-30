using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class AI_Enemy1 : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack, Dead }

    [Header("AI Attack")]
    [SerializeField] private bool _isSittingEnemy;
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
    private bool _isBlockingState = false;
    private bool _alreadyTriggeredFromIdle = false;

    private int _chaseSoundEntityID = -1;

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

    private void CheckStateTransitions()
    {
        if (_isBlockingState) return;


        float distanceToPlayer = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);


        switch (_currentState)
        {
            case EnemyState.Chase:
                if (distanceToPlayer <= _attackRange && !_isAttacking && !_isBlockingState)
                {
                    ChangeState(EnemyState.Attack);
                }
                break;

            case EnemyState.Attack:
                if (distanceToPlayer > _attackRange && !_isAttacking)
                {
                    Debug.Log("Attack but change yo: " + _isAttacking);
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
        if (_alreadyTriggeredFromIdle) return;

        if (other.GetComponent<PlayerController>() != null)
        {
            if (_currentState == EnemyState.Idle && !_isBlockingState)
            {
                ChangeState(EnemyState.Chase);
                DisableMovement();
                _alreadyTriggeredFromIdle = true;
                Debug.Log("Player detected. Entering chase state.");
            }
            else if (_currentState == EnemyState.Idle && _isBlockingState)
            {
                ChangeState(EnemyState.Chase);
                DisableMovement();
                _alreadyTriggeredFromIdle = true;
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
        if (_currentState == EnemyState.Chase && _chaseSoundEntityID != -1)
        {
            SoundManager.Instance.StopOneShotByEntityID(_chaseSoundEntityID);
            _chaseSoundEntityID = -1;
            Debug.Log("Stopped chase sound using ID.");
        }

        _currentState = newState;
        Debug.Log($"Enemy state changed to: {newState} + {_isBlockingState}");

        switch (newState)
        {
            case EnemyState.Idle:
                _enemy.EnemyMovement.DisableMovement(true);

                if (_isSittingEnemy)
                {
                    _animator.SetTrigger("IdleSitting");
                    _isBlockingState = true;
                    _enemy.EnemyMovement.DisableMovement(true);
                    GetComponent<NavMeshAgent>().enabled = false;

                }
                else
                {
                    _animator.SetTrigger("Idle");
                }
                break;

            case EnemyState.Chase:
                _chaseSoundEntityID = SoundManager.Instance.PlaySound("sfx_enemyrun_indoors");
                if (!_isBlockingState)
                    _enemy.EnemyMovement.DisableMovement(false);

                if (!_alreadyTriggeredFromIdle && _isSittingEnemy)
                {
                    Debug.Log("First time");
                    _enemy.EnemyMovement.DisableMovement(true);
                    Util.WaitForSeconds(this,() =>
                    {
                        GetComponent<NavMeshAgent>().enabled = true;

                    }, 1f);
                }

                _enemy.EnemyMovement.SetRotationControl(true);
                _enemy.EnemyMovement.MoveTo(PlayerController.Instance.transform.position);
                _isAttacking = false;
                _animator.SetTrigger("Chasing");
                break;

            case EnemyState.Attack:
                _enemy.EnemyMovement.DisableMovement(true);
                _enemy.EnemyMovement.SetRotationControl(false);
                _isAttacking = true;
                _animator.SetTrigger("Attack");
                break;

            case EnemyState.Dead:
                _enemy.EnemyMovement.DisableMovement(true);
                // _animator.SetTrigger("Die");
                break;

        }
    }

    public void DealDamage()
    {
        float distanceToPlayer = Vector3.Distance(PlayerController.Instance.transform.position, transform.position);

        if (distanceToPlayer <= _attackRange)
        {
            Vector3 direction = PlayerController.Instance.transform.position - transform.position;
            SanityManager.Instance.DecreaseSanity(_enemy.AttackDamage, direction);

        }
    }
    public void StartChasingPlayer()
    {
        ChangeState(EnemyState.Chase);
    }
    public void SetAttackFinished()
    {
        Util.WaitForSeconds(this, () =>
        {
            _isAttacking = false;
            _nextAttackTime = Time.time + _attackCooldown;
            _enemy.EnemyMovement.DisableMovement(false);
            ChangeState(EnemyState.Chase);

            Debug.Log("Attack sequence finished, check for next state.");
        }, 0.5f);

    }

    public void SetNotBlockingState()
    {
        Debug.Log("Se tnot blocking state");
        _isBlockingState = false;
    }

    public void DisableMovement()
    {
        _enemy.EnemyMovement.DisableMovement(true);
    }

    public void PlayAttackSwingSound()
    {
        // Call the sound exactly when the animation event is triggered
        SoundManager.Instance.PlaySound("sfx_sword_swingsh_swing");
        Debug.Log("Attack Sound Triggered by Animation Event!");
    }
}
