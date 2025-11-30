using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Ragdoll")]
    public GameObject ragdollPrefab;
    private GameObject ragdoll;
    public float strength = 10000f;

    [Header("Hit VFX")]
    public ParticleSystem hitParticle;

    public void Death(Vector3 hitPoint)
    {
        // Play hit particle effect and detach from parent so it continues after enemy is destroyed
        if (hitParticle != null)
        {
            hitParticle.transform.SetParent(null); // Detach from enemy
            hitParticle.Stop();
            hitParticle.Clear();
            hitParticle.Play();
            Destroy(hitParticle.gameObject, 2f); // Destroy particle after 2 seconds
        }

        // Enemy Death Spawn Ragdoll
        ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        ragdoll.GetComponent<EnemyRagdoll>().Ragdoll((transform.position - hitPoint), strength);
        Destroy(gameObject);
    }
}
