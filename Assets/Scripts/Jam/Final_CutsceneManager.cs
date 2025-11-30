using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Final_CutsceneManager : MonoBehaviour
{
    private PlayableDirector _director;
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
        Debug.Log("Enabling animator and playing cutscene animation from start");

        // Play dialogue
        Debug.Log("Triggered dialogue ID 6");
       
    }
}
