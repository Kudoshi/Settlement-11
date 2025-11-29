using System.Runtime.CompilerServices;
using UnityEngine;

public class Enemy_test : MonoBehaviour
{
    private Vector3 hitPoint;
    public GameObject ragdollPrefab;
    private GameObject ragdoll;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy collided with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("enemy hit sword");
            hitPoint = collision.contacts[0].point;
            ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            ragdoll.GetComponent<EnemyRagdoll>().Ragdoll((transform.position - hitPoint), 10000);
            Destroy(gameObject);
        }
    }
}
