using Kudoshi.Utilities;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Intro_CutsceneManager : Singleton<Intro_CutsceneManager>
{
    [SerializeField] private GameObject phoneObject;
    [SerializeField] private Material blankMaterial;
    [SerializeField] private Material callingMaterial;
    [SerializeField] private Material onCallMaterial;

    private MeshRenderer phoneRenderer;
    private Animator animator;
    private PlayableDirector _director;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _director = GetComponent<PlayableDirector>();
        phoneObject.SetActive(false);

        // Disable animator on awake so animation doesn't auto-play
        //if (animator != null)
        //{
        //    animator.enabled = false;
        //}
    }

    private void Start()
    {
        if (phoneObject != null)
        {
            phoneRenderer = phoneObject.GetComponent<MeshRenderer>();
        }
    }


    public void EnableAndPlayAnimation()
    {
        if (animator != null)
        {
            Debug.Log("Enabling intro animator and playing animation");
            animator.enabled = true;
            animator.Play(0, 0, 0f); // Play default state from beginning
        }
    }

    public void SetPhoneBlank()
    {
        if (phoneRenderer != null && blankMaterial != null)
        {
            phoneRenderer.material = blankMaterial;
        }
    }

    public void SetPhoneCalling()
    {
        if (phoneRenderer != null && callingMaterial != null)
        {
            phoneRenderer.material = callingMaterial;
        }
    }

    public void SetPhoneOnCall()
    {
        if (phoneRenderer != null && onCallMaterial != null)
        {
            //DialogueManager.Instance.PlayDialogueID(6);
            phoneRenderer.material = onCallMaterial;
        }
    }

    public void CutsceneEnd()
    {
        Debug.Log("Cutscene end");
        SceneManager.LoadScene("City");
    }

    public void CutsceneStart()
    {
        _director.Play();
        phoneObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CutsceneStart();
        }
    }

    public void PlayDialogue()
    {
        DialogueManager.Instance.PlayDialogueID(6);
    }
}

