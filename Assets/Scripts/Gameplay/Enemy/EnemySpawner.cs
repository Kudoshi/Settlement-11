using UnityEngine;
using Kudoshi.Utilities;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool _isTimerBasedSpawner;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _delayBeforeStartSpawning;
    [SerializeField] private float _durationSpawning = -1f;
    [SerializeField] private float _spawnCooldown;
    [SerializeField] private EnemySpawnList[] _spawnList;
    [SerializeField] private SO_Enemy _enemySO;

    private bool _spawnActive;
    private Coroutine _spawnCr;
    public void StartEnemyWave()
    {
        _spawnActive = true;

        if (_spawnCr != null)
        {
            StopCoroutine(_spawnCr);
        }

        _spawnCr = StartCoroutine(Cr_EnemyWaveSpawn());
    }

    public void StopEnemyWave()
    {
        _spawnActive = false;
        if (_spawnCr != null)
        {
            StopCoroutine(_spawnCr);
            _spawnCr = null;
        }
    }


    private IEnumerator Cr_EnemyWaveSpawn()
    {
        yield return new WaitForSeconds(_delayBeforeStartSpawning);
        float totalTime = 0;
        float prevTime = Time.time;
        while (_spawnActive && (!_isTimerBasedSpawner || totalTime <= _durationSpawning))
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnCooldown);

            if (_isTimerBasedSpawner)
            {
                totalTime += Time.time - prevTime;
                prevTime = Time.time;
            }
        }

        _spawnActive = false;
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < _spawnList.Length; i++)
        {
            for (int j = 0; j < _spawnList[i].EnemyAmount; j++)
            {
                // Get random location
                int randomSpawnIdx = Random.Range(0, _spawnPoints.Length);
                Vector3 position = _spawnPoints[randomSpawnIdx].position;
                Enemy enemy = _enemySO.GetEnemyObject(_spawnList[i].EnemyToSpawn);
                Enemy spawned = Instantiate(enemy, position, Quaternion.identity);

                Util.WaitForSeconds(this, () =>
                {
                    spawned.Enemy1?.StartChasingPlayer();
                    spawned.Enemy2?.StartChasingPlayer();

                }, .1f);
                
            }
        }
    }
}

//public class EnemySpawnWaveConfig
//{
//    public float SpawnCooldown;
//    public float SpawnDuration;
//    public EnemySpawnInstance SpawnInstance;
//}

[System.Serializable]
public class EnemySpawnList
{
    public int EnemyAmount;
    public EnemyType EnemyToSpawn;
}