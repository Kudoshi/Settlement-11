
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private int _dialogueIdx;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.PlayDialogueID(_dialogueIdx);
            Destroy(gameObject);
        }
    }
}