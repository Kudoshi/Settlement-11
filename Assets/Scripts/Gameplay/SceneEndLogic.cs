using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneEndLogic : MonoBehaviour
{
    [Header("Transition")]
    public CanvasGroup transitionCanvas;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    [Header("Scene Settings")]
    public string nextSceneName;
    public float waitTimeAfterEnemiesDead = 2f;
    public bool onlyFadeIn = false;

    private bool hasTransitioned = false;

    private void Start()
    {
        // Fade in from black on start
        if (transitionCanvas != null)
        {
            transitionCanvas.alpha = 1f;
            transitionCanvas.DOFade(0f, fadeInDuration);
        }
    }

    private void Update()
    {
        if (onlyFadeIn) return;

        if (hasTransitioned)
            return;

        // Check if all enemies are dead
        if (AreAllEnemiesDead())
        {
            hasTransitioned = true;
            TransitionToNextScene();
        }
    }

    private bool AreAllEnemiesDead()
    {
        // Find all enemies with EnemyHealth component
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        return enemies.Length == 0;
    }

    private void TransitionToNextScene()
    {
        Debug.Log("All enemies dead! Transitioning to next scene...");

        // Wait a bit, then fade to black and load next scene
        DOVirtual.DelayedCall(waitTimeAfterEnemiesDead, () =>
        {
            if (transitionCanvas != null)
            {
                transitionCanvas.DOFade(1f, fadeOutDuration).OnComplete(() =>
                {
                    LoadNextScene();
                });
            }
            else
            {
                LoadNextScene();
            }
        });
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("SceneEndLogic: Next scene name is not set!");
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
