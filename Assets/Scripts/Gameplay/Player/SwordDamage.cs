using Unity.Collections;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public ParticleSystem swordVFX;

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

    public void SpawnSwordVFX()
    {
        swordVFX.Play();
    }
}
