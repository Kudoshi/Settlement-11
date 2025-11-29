using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public GameObject ragdollPrefab;

    private GameObject ragdoll;
    private Vector3 hitPoint;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Sword collided with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Sword hit enemy");
            hitPoint = collision.contacts[0].point;
            ragdoll = Instantiate(ragdollPrefab, collision.transform.position, collision.transform.rotation);
            ragdoll.GetComponent<EnemyRagdoll>().Ragdoll((collision.transform.position - hitPoint).normalized, 500);
        }
    }
}
