using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public GameObject ragdollPrefab;
    private GameObject ragdoll;
    public float strength = 10000f;

    public void Death(Vector3 hitPoint)
    {
        // Enemy Death Spawn Ragdoll
        ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        ragdoll.GetComponent<EnemyRagdoll>().Ragdoll((transform.position - hitPoint), strength);
        Destroy(gameObject);
    }
    
}
