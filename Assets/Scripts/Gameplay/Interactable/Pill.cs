using UnityEngine;

public class Pill : MonoBehaviour
{
    [SerializeField] private Outline outline; // optional, if you use an outline script

    private void Start()
    {
        // Auto-detect outline if not manually assigned
        if (outline == null)
            outline = GetComponent<Outline>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        ConsumePill();
        Destroy(gameObject);
    }

    private void ConsumePill()
    {
        Debug.Log("Increase Sanity");
        SanityManager.Instance.IncreaseSanity(20f);
    }

}
