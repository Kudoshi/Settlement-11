using UnityEngine;
using DG.Tweening;

public class PillPickup : MonoBehaviour
{
    [Header("Float Animation")]
    public float floatHeight = 0.5f;
    public float floatSpeed = 1f;

    [Header("Rotation")]
    public bool rotatePickup = true;
    public float rotationSpeed = 90f;

    [Header("Collection")]
    public float collectRange = 2f;
    public float sanityRestore = 20f;
    public LayerMask playerLayer;

    [Header("Juice Effects")]
    public float collectScaleMultiplier = 1.5f;
    public float collectDuration = 0.3f;
    public Ease collectEase = Ease.OutBack;

    private Vector3 startPosition;
    private bool isCollected = false;

    private void Start()
    {
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
                .SetLoops(-1, LoopType.Restart);
        }
    }

    private void Update()
    {
        if (isCollected) return;

        // Check if player is near
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collectRange, playerLayer);

        if (hitColliders.Length > 0)
        {
            CollectPickup();
        }
    }

    private void CollectPickup()
    {
        if (isCollected) return;
        isCollected = true;

        // Kill existing tweens
        transform.DOKill();

        // Juicy collection animation
        Sequence collectSequence = DOTween.Sequence();

        // Scale up
        collectSequence.Append(transform.DOScale(transform.localScale * collectScaleMultiplier, collectDuration * 0.4f)
            .SetEase(collectEase));

        // Move up slightly
        collectSequence.Join(transform.DOMoveY(transform.position.y + 0.5f, collectDuration * 0.4f)
            .SetEase(Ease.OutQuad));

        // Scale down and fade out
        collectSequence.Append(transform.DOScale(0f, collectDuration * 0.6f)
            .SetEase(Ease.InBack));

        // Add camera shake on collect
        collectSequence.AppendCallback(() =>
        {
            if (PlayerCamera.Instance != null)
            {
                PlayerCamera.Instance.Shake(0.1f, 0.1f);
            }
        });

        // Restore sanity
        if (SanityManager.Instance != null)
        {
            SanityManager.Instance.IncreaseSanity(sanityRestore);
        }

        // Destroy after animation
        collectSequence.OnComplete(() => Destroy(gameObject));
    }

    private void OnDrawGizmosSelected()
    {
        // Draw collection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRange);
    }

    private void OnDestroy()
    {
        // Clean up tweens
        transform.DOKill();
    }
}
