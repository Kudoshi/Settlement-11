using UnityEngine;
using DG.Tweening;
using System.Collections;

public class RoomObjective : MonoBehaviour
{
    public enum ObjectiveType
    {
        R1,
        R2,
        R3,
        R4
    }

    [SerializeField] private ObjectiveType objectiveType;
    [SerializeField] private GameObject targetToDisable;
    [SerializeField] private CanvasGroup worldSpaceUI;
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private Light[] lightsToDim;
    [SerializeField] private float dimIntensity = 0.3f;
    [SerializeField] private float dimDuration = 2f;
    [SerializeField] private Color fogColor = Color.black;
    [SerializeField] private float fogDensity = 0.05f;
    [SerializeField] private float fogTransitionSpeed = 1f;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private CanvasGroup transitionCanvas;
    [SerializeField] private float r4FadeDuration = 1f;
    [SerializeField] private float r4FadeWaitTime = 3f;
    [SerializeField] private AudioSource collectAudioSource;
    [SerializeField] private GameObject cutsceneObject;

    private static bool r1Opened = false;
    private static bool r2Completed = false;
    private bool isInteracted = false;
    private Transform playerTransform;
    private bool isLookingAt = false;
    private Color originalFogColor;
    private float originalFogDensity;

    private void Start()
    {
        if (worldSpaceUI != null)
        {
            worldSpaceUI.alpha = 0f;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
    }

    private void Update()
    {
        if (isInteracted || playerTransform == null || worldSpaceUI == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = distance <= detectionRadius;

        if (inRange && !isLookingAt)
        {
            worldSpaceUI.alpha = Mathf.Lerp(worldSpaceUI.alpha, 1f, Time.deltaTime * 8f);
        }
        else
        {
            worldSpaceUI.alpha = Mathf.Lerp(worldSpaceUI.alpha, 0f, Time.deltaTime * 8f);
        }
    }

    public void SetLookingAt(bool looking)
    {
        isLookingAt = looking;
    }

    public bool CanInteract()
    {
        if (isInteracted) return false;

        if (objectiveType == ObjectiveType.R1)
        {
            return true;
        }
        else if (objectiveType == ObjectiveType.R2)
        {
            return r1Opened;
        }
        else if (objectiveType == ObjectiveType.R3)
        {
            return r1Opened;
        }
        else if (objectiveType == ObjectiveType.R4)
        {
            return r2Completed;
        }

        return false;
    }

    public void Interact(RoomPlayerController player)
    {
        if (!CanInteract()) return;

        isInteracted = true;

        if (objectiveType == ObjectiveType.R1)
        {
            r1Opened = true;

            foreach (Light light in lightsToDim)
            {
                if (light != null)
                {
                    light.DOIntensity(dimIntensity, dimDuration);
                }
            }

            StartCoroutine(TransitionFog());
        }
        else if (objectiveType == ObjectiveType.R2)
        {
            if (targetToDisable != null)
            {
                targetToDisable.SetActive(false);
            }
            player.EnableHeldPills();
            r2Completed = true;

            if (collectAudioSource != null)
            {
                collectAudioSource.Play();
            }
        }
        else if (objectiveType == ObjectiveType.R3)
        {

        }
        else if (objectiveType == ObjectiveType.R4)
        {
            if (cutsceneObject != null)
            {
                cutsceneObject.SetActive(true);
            }

            StartCoroutine(HandleR4Transition());
        }
    }

    private IEnumerator HandleR4Transition()
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.alpha = 0f;
            transitionCanvas.DOFade(1f, r4FadeDuration).OnComplete(() =>
            {
                if (playerObject != null)
                {
                    playerObject.SetActive(false);
                }
            });
        }

        yield return null;
    }

    private IEnumerator TransitionFog()
    {
        float elapsed = 0f;
        Color startColor = RenderSettings.fogColor;
        float startDensity = RenderSettings.fogDensity;

        while (elapsed < fogTransitionSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fogTransitionSpeed;

            RenderSettings.fogColor = Color.Lerp(startColor, fogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, fogDensity, t);

            yield return null;
        }

        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }
}
