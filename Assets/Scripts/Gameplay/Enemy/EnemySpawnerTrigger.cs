
using UnityEngine;

public class EnemySpawnerTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawner _spawner;
    [SerializeField] private bool _isEntrySpawnerTrigger;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isEntrySpawnerTrigger)
                _spawner.StartEnemyWave();
            else _spawner.StopEnemyWave();

            Destroy(gameObject);
        }
    }
}