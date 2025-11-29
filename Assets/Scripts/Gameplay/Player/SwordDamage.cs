using Unity.Collections;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public BoxCollider sword_collider;

    private void Start()
    {
        sword_collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
       
    }

    private void OnTriggerEnter(Collider collide)
    {
        if (collide.CompareTag("Enemy"))
        {
            if (collide.gameObject.GetComponent<Testing_EnemyHealth>() != null)
            {
                collide.gameObject.GetComponent<Testing_EnemyHealth>().TakeDamage();
            }
        }
    }
}
