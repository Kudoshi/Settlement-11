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
        // Play hit particle effect - stop first, then restart to ensure it plays
        if (hitParticle != null)
        {
            hitParticle.Stop();
            hitParticle.Clear();
            hitParticle.Play();
        }

        // Enemy Death Spawn Ragdoll
        ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        ragdoll.GetComponent<EnemyRagdoll>().Ragdoll((transform.position - hitPoint), strength);
        Destroy(gameObject);
    }
}
