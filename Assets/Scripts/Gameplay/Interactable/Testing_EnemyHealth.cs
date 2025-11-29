using UnityEngine;

public class Testing_EnemyHealth : MonoBehaviour
{
    public void TakeDamage()
    {
        Debug.Log("Enemy took damage!");
        Destroy(gameObject);
    }
}
