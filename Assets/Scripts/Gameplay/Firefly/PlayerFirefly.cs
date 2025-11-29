using Kudoshi.Utilities;
using System;
using UnityEngine;

public class PlayerFirefly : Singleton<PlayerFirefly>
{
    [SerializeField] private int _fireflyMax;

    private int _fireflyCounter;

    public int FireflyCounter => _fireflyCounter;
    public event Action OnFireflyChanged;

    private void Awake()
    {
        _fireflyCounter = 0;
    }

    public void AdjustFireflies(int firefly)
    {
        _fireflyCounter += firefly;

        Debug.Log("Firefly Counter: " + _fireflyCounter);

        OnFireflyChanged?.Invoke();   
    }
}
