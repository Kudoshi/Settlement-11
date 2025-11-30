using UnityEngine;

public class Final_CutsceneManager : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // Disable animator on awake so animation doesn't auto-play
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    public void PlayCutsceneAnimation()
    {
        if (animator != null)
        {
            Debug.Log("Enabling animator and playing cutscene animation from start");
            animator.enabled = true;
            animator.Play("IntroRoom", 0, 0f);

            // Play dialogue
            DialogueManager.Instance.PlayDialogueID(6);
            Debug.Log("Triggered dialogue ID 6");
        }
        else
        {
            Debug.LogWarning("Animator component not found on Final_CutsceneManager!");
        }
    }
}
