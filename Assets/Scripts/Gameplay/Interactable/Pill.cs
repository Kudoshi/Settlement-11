using UnityEngine;
using DG.Tweening;

public class Pill : MonoBehaviour
{
    [SerializeField] private Outline outline; // optional, if you use an outline script

    [Header("Float Animation")]
    public float floatHeight = 0.3f;
    public float floatSpeed = 1.5f;

    [Header("Rotation")]
    public bool rotatePickup = true;
    public float rotationSpeed = 2f;

    [Header("Collection")]
    public float sanityRestore = 20f;

    [Header("Juice Effects")]
    public float collectScaleMultiplier = 1.5f;
    public float collectDuration = 0.4f;

    private Vector3 startPosition;
    private bool isCollected = false;

    private void Start()
    {
        // Auto-detect outline if not manually assigned
        if (outline == null)
            outline = GetComponent<Outline>();

        startPosition = transform.position;

        // Float up and down animation
        transform.DOMoveY(startPosition.y + floatHeight, floatSpeed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Rotate continuously
        if (rotatePickup)
        {
            transform.DORotate(new Vector3(0, 360, 0), rotationSpeed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isCollected)
            return;

        ConsumePill();
    }

    private void ConsumePill()
    {
        SoundManager.Instance.PlaySound("sfx_pills_pickup");
        isCollected = true;

        Debug.Log("Increase Sanity");
        SanityManager.Instance.IncreaseSanity(sanityRestore);

        // Kill existing tweens
        transform.DOKill();

        // Juicy collection animation
        Sequence collectSequence = DOTween.Sequence();

        // Scale up with bounce
        collectSequence.Append(transform.DOScale(transform.localScale * collectScaleMultiplier, collectDuration * 0.4f)
            .SetEase(Ease.OutBack));

        // Move up slightly
        collectSequence.Join(transform.DOMoveY(transform.position.y + 0.5f, collectDuration * 0.4f)
            .SetEase(Ease.OutQuad));

        // Scale down to zero
        collectSequence.Append(transform.DOScale(0f, collectDuration * 0.6f)
            .SetEase(Ease.InBack));

        // Camera shake on collect
        collectSequence.AppendCallback(() =>
        {
            if (PlayerCamera.Instance != null)
            {
                PlayerCamera.Instance.Shake(0.15f, 0.1f);
            }
        });

        // Destroy after animation
        collectSequence.OnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        // Clean up tweens
        transform.DOKill();
    }
}
