using UnityEngine;
using DG.Tweening;

public class TrainLoop : MonoBehaviour
{
    public float startZ = 71.96f;
    public float endZ = 25.85f;
    public float travelTime = 2f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float knockbackForce = 10f;

    private void Start()
    {
        // Start position
        Vector3 pos = transform.position;
        pos.z = startZ;
        transform.position = pos;

        // Setup colliders for damage
        SetupColliders();

        // Random wait then start
        float randomWait = Random.Range(minWaitTime, maxWaitTime);
        DOVirtual.DelayedCall(randomWait, () => MoveTrain());
    }

    private void SetupColliders()
    {
        // Get all box colliders in children
        BoxCollider[] colliders = GetComponentsInChildren<BoxCollider>();

        foreach (BoxCollider col in colliders)
        {
            if (col.isTrigger)
            {
                // Add train collider component
                TrainCollider tc = col.gameObject.AddComponent<TrainCollider>();
                tc.knockbackForce = knockbackForce;
            }
        }
    }

    private void MoveTrain()
    {
        // Move to end
        Vector3 endPos = transform.position;
        endPos.z = endZ;

        transform.DOMove(endPos, travelTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // Flip 180
                transform.rotation = Quaternion.Euler(0, 180, 0);

                // Move back to start
                Vector3 startPos = transform.position;
                startPos.z = startZ;

                transform.DOMove(startPos, travelTime)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        // Rotate back to 0
                        transform.rotation = Quaternion.Euler(0, 0, 0);

                        // Random wait then loop again
                        float randomWait = Random.Range(minWaitTime, maxWaitTime);
                        DOVirtual.DelayedCall(randomWait, () => MoveTrain());
                    });
            });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}

public class TrainCollider : MonoBehaviour
{
    public float knockbackForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get knockback direction (horizontal only, no flying to heaven)
            Vector3 knockbackDir = (other.transform.position - transform.position);
            knockbackDir.y = 0; // No vertical knockback
            knockbackDir = knockbackDir.normalized;

            // Reduce health to half
            if (SanityManager.Instance != null)
            {
                float currentHealth = SanityManager.Instance.currentSanity;
                float damage = currentHealth * 0.5f; // Half health
                SanityManager.Instance.DecreaseSanity(damage, knockbackDir * knockbackForce);
            }

            // Camera shake on train hit
            if (PlayerCamera.Instance != null)
            {
                PlayerCamera.Instance.Shake(0.5f, 0.3f);
            }

            Debug.Log("Train hit player!");
        }
    }
}
