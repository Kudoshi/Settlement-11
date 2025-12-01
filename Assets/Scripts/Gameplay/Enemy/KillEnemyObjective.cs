using Kudoshi.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillEnemyObjective : Singleton<KillEnemyObjective>
{
    [SerializeField] private TextMeshProUGUI _objText;
    [SerializeField] private int _maxEnemyKillAmountToWin;
    private int _enemyKillCount;

    private void Start()
    {
        _objText.text = $"Kill {_enemyKillCount} / {_maxEnemyKillAmountToWin}";
    }

    public void EnemyKilled()
    {
        _enemyKillCount++;
        _objText.text = $"Kill {_enemyKillCount} / {_maxEnemyKillAmountToWin}";

        if (_enemyKillCount >= _maxEnemyKillAmountToWin)
        {
            Debug.Log("End Game");
            SceneManager.LoadScene("Outro");
        }
    }
}
