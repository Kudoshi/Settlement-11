using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private Vector3 hitPoint;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Sword hit enemy");
            hitPoint = collision.contacts[0].point;
            collision.gameObject.GetComponent<EnemyHealth>().Death(hitPoint);
        }
    }
}
