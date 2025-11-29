
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Enemy", menuName = "Scriptable Objects/SO_Enemy")]
public class SO_Enemy : ScriptableObject
{
    [SerializeField] private SerializedDictionary<EnemyType, Enemy> _enemyObjects;

    public Enemy GetEnemyObject(EnemyType enemyType)
    {
        return _enemyObjects[enemyType];
    }
}

