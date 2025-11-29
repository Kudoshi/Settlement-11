using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyMovement EnemyMovement;
    public AI_Enemy1 Enemy1;
    public AI_Enemy2 Enemy2;

    [SerializeField] private float _attackDamage = 10f;

    public float AttackDamage { get => _attackDamage; }

    void Start()
    {
        if (EnemyMovement == null)
            EnemyMovement = GetComponent<EnemyMovement>();
        if (Enemy1 == null)
            Enemy1 = GetComponent<AI_Enemy1>();
        if (Enemy2 == null)
            Enemy2 = GetComponent<AI_Enemy2>();

    }

    void Update()
    {
        
    }
}
