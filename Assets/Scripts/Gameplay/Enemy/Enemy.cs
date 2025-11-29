using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyMovement EnemyMovement;
    public AI_Enemy1 Enemy1;

    [SerializeField] private float _attackDamage = 10f;

    public float Attackdamage { get => _attackDamage; }

    void Start()
    {
        if (EnemyMovement == null)
            EnemyMovement = GetComponent<EnemyMovement>();
        if (Enemy1 == null)
            Enemy1 = GetComponent<AI_Enemy1>();

    }

    void Update()
    {
        
    }
}
