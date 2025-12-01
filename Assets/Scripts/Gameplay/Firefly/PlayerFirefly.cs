using Kudoshi.Utilities;
using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerFirefly : Singleton<PlayerFirefly>
{
    [SerializeField] private int _fireflyMax = 20;
    [SerializeField] private Image _fireflyImg;
    [SerializeField] private GameObject _skillImg;
    [SerializeField] private Sprite[] _fireflySprites;

    [Header("Juice Settings")]
    [SerializeField] private float _shakeAmount = 5f;
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private Color _fullColor = new Color(1f, 1f, 0.5f); // Bright Yellow
    [SerializeField] private float _pulseDuration = 1f;
    [SerializeField] private float _pulseScale = 1.1f;

    private Vector3 _originalScale;
    private Color _originalColor;
    private RectTransform _fireflyRect;
    
    private int _fireflyCounter;
    public event Action OnFireflyChanged;

    public int FireflyCounter { get => _fireflyCounter; }

    private void Awake()
    {
        _fireflyCounter = 0;
    }

    private void Start()
    {
        if (_fireflyImg != null)
        {
            _fireflyRect = _fireflyImg.GetComponent<RectTransform>();
            _originalScale = _fireflyRect.localScale;
            _originalColor = _fireflyImg.color;
        }

        ImageCounter();
        _fireflyImg.enabled = true;
        _skillImg.SetActive(false);
    }

    public void AdjustFireflies(int firefly)
    {
        _fireflyCounter += firefly;
        _fireflyCounter = Mathf.Clamp(_fireflyCounter, 0, _fireflyMax -1);
        ImageCounter();

        if (firefly > 0 && _fireflyRect != null)
        {
            // Shake effect (using complete to stop previous shake if rapid fire)
            _fireflyRect.DOComplete(); 
            _fireflyRect.DOShakeAnchorPos(_shakeDuration, _shakeAmount);
        }

        // Check for Full State
        if (_fireflyCounter >= _fireflyMax - 1)
        {
            _skillImg.SetActive(true);
            
            // Start Pulse if not already active
            // We check if the unique ID for pulsing is playing or just check if scale is tweening
            if (!DOTween.IsTweening(_fireflyRect.localScale)) 
            {
                _fireflyRect.DOScale(_originalScale * _pulseScale, _pulseDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                _fireflyImg.DOColor(_fullColor, _pulseDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }
        }

        OnFireflyChanged?.Invoke();
    }

    public bool UseAllFireflies()
    {
        if (_fireflyCounter >= _fireflyMax - 1) 
        {
            Debug.Log("All fireflies used. Counter reset to 0.");
            _fireflyCounter = 0;
            
            // Kill Tweens and Reset Visuals
            _fireflyRect.DOKill();
            _fireflyImg.DOKill();

            _fireflyImg.enabled = true;
            _fireflyImg.color = _originalColor;
            _fireflyRect.localScale = _originalScale;
            
            _skillImg.SetActive(false);
            ImageCounter();
            return true;
        }
        ImageCounter();
        return false;
    }

    private void ImageCounter()
    {
        if (_fireflyCounter < _fireflySprites.Length)
            _fireflyImg.sprite = _fireflySprites[_fireflyCounter];
    }
}