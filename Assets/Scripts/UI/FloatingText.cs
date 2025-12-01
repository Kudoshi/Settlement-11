using UnityEngine;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    public float floatAmount = 10f;
    public float floatDuration = 1.5f;

    private RectTransform _rectTransform;
    private Vector2 _originalPosition;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalPosition = _rectTransform.anchoredPosition;

        _rectTransform.DOAnchorPosY(_originalPosition.y + floatAmount, floatDuration / 2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}