using Kudoshi.Utilities;
using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFirefly : Singleton<PlayerFirefly>
{
    [SerializeField] private int _fireflyMax = 20;
    [SerializeField] private Image _fireflyImg;
    [SerializeField] private GameObject _skillImg;
    [SerializeField] private Sprite[] _fireflySprites;

    private int _fireflyCounter;

    public int FireflyCounter => _fireflyCounter;
    public event Action OnFireflyChanged;

    private void Awake()
    {
        _fireflyCounter = 0;
    }

    private void Start()
    {
        ImageCounter();
        _fireflyImg.enabled = true;
        _skillImg.SetActive(false);
    }

    public void AdjustFireflies(int firefly)
    {
        _fireflyCounter += firefly;
        _fireflyCounter = Mathf.Clamp(_fireflyCounter, 0, _fireflyMax -1);
        ImageCounter();
        Debug.Log("Firefly Counter: " + _fireflyCounter);

        OnFireflyChanged?.Invoke();  
    }

         
    public bool UseAllFireflies()
    {
        if (_fireflyCounter >= _fireflyMax - 1) 
        {
            Debug.Log("All fireflies used. Counter reset to 0.");
            _fireflyCounter = 0;
            _fireflyImg.enabled = true;
            _skillImg.SetActive(false);
            ImageCounter();
            return true;
        }
        ImageCounter();
        return false;
    }

    private void ImageCounter()
    {
        _fireflyImg.sprite = _fireflySprites[_fireflyCounter];
    }

    private void Update()
    {
        if (_fireflyCounter >= _fireflyMax - 1)
        {
            _skillImg.SetActive(true);
            _fireflyImg.enabled = false;
        }
    }
}
