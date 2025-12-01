using DG.Tweening;
using Kudoshi.Utilities;
using NUnit.Framework;
using System;
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
    public event Action OnFireflyChanged;

    private bool _canSkill;

    public int FireflyCounter { get => _fireflyCounter; }

    private void Awake()
    {
        _fireflyCounter = 0;
    }

    private void Start()
    {
        ImageCounter();
        _fireflyImg.enabled = true;
        _canSkill = false;
        _skillImg.SetActive(false);
    }

    public void AdjustFireflies(int firefly)
    {
        _fireflyCounter += firefly;
        _fireflyCounter = Mathf.Clamp(_fireflyCounter, 0, _fireflyMax);

        if (FireflyCounter >= _fireflyMax && !_canSkill)
        {
            _canSkill = true;
            _skillImg.SetActive(true);
            _fireflyImg.enabled = false;
            SoundManager.Instance.PlaySound("sfx_skill_gain");
            SoundManager.Instance.PlaySound("sfx_skill_up");
            ShakeViolentThenNormalize();
        }
        else
        {
            ImageCounter();

        }

        OnFireflyChanged?.Invoke();
    }

    public bool UseAllFireflies()
    {
        if (_fireflyCounter >= _fireflyMax) 
        {
            Debug.Log("All fireflies used. Counter reset to 0.");
            _fireflyCounter = 0;
            _fireflyImg.enabled = true;
            _skillImg.SetActive(false);
            _canSkill = false;
            ImageCounter();
            return true;
        }
        ImageCounter();
        return false;
    }

    private void ImageCounter()
    {
        int spriteCount = _fireflySprites.Length;

        if (spriteCount == 0 || _fireflyImg == null || _canSkill)
            return;

        float normalized = (float)_fireflyCounter / _fireflyMax;

        int index = Mathf.FloorToInt(normalized * (spriteCount - 1));
        index = Mathf.Clamp(index, 0, spriteCount - 1);

        _fireflyImg.sprite = _fireflySprites[index];

        _fireflyImg.transform.DOKill(); // stop any previous pop

        _fireflyImg.transform
            .DOScale(1.2f, 0.1f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                _fireflyImg.transform
                    .DOScale(1f, 0.1f)
                    .SetEase(Ease.InOutSine);
            });
    }

    private void ShakeViolentThenNormalize()
    {
        Transform t = _skillImg.transform;
        t.DOKill();

        // Instantly pop bigger
        t.localScale = Vector3.one * 2.0f;

        // Violent shake
        t.DOShakePosition(2.0f, 50f, 50, 90, false, true)
         .SetEase(Ease.OutQuad);

        // Slowly go back to normal scale
        t.DOScale(1.5f, 2f)
         .SetDelay(0.1f)
         .SetEase(Ease.OutExpo);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.I))
        {
            AdjustFireflies(5);
        }
    }
#endif
}