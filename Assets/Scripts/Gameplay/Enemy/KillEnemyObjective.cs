using Kudoshi.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillEnemyObjective : Singleton<KillEnemyObjective>
{
    [SerializeField] private int _maxEnemyKillAmountToWin;
    private int _enemyKillCount;

    public void EnemyKilled()
    {
        _enemyKillCount++;

        if (_enemyKillCount >= _maxEnemyKillAmountToWin)
        {
            Debug.Log("End Game");
            SceneManager.LoadScene("Outro");
        }
    }
}
