using DG.Tweening;
using Kudoshi.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SanityManager : Singleton<SanityManager>
{
    [Header("Settings")]
    public Image sanityImg; 
    public float currentSanity;
    public float maxSanity = 100f;
    public float slowDuration;
    public CanvasGroup canvasGroup;

    private void Start()
    {
        currentSanity = maxSanity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DecreaseSanity(10f);
        }

        InternalSanityUpdate();
    }

    private void InternalSanityUpdate()
    {
        currentSanity -= 3f * Time.deltaTime;
        sanityImg.fillAmount = Mathf.Lerp(sanityImg.fillAmount, currentSanity / maxSanity, Time.deltaTime * 10f);

        if (currentSanity <= 0) GameOver();
    }

    private void UpdateSanity(float current_sanity, float max_sanity)
    {
        currentSanity = current_sanity;
        maxSanity = max_sanity;

        //Debug.Log("Current sanity: " + currentSanity);
    }

    public void IncreaseSanity(float sanity)
    {
        currentSanity += sanity;

        if (currentSanity > maxSanity)
        {
            currentSanity = maxSanity;
        }
        UpdateSanity(currentSanity, maxSanity);
    }

    public void DecreaseSanity(float sanity)
    {
        currentSanity -= sanity;

        if (currentSanity <= 0)
        {
            currentSanity = 0;
            GameOver();

        }

        UpdateSanity(currentSanity, maxSanity);
    }

    private void GameOver()
    {
        StartCoroutine(SlowGame(slowDuration));
        canvasGroup.DOFade(1f, slowDuration);
    }

    public IEnumerator SlowGame(float duration)
    {
        PlayerInteractable.Instance.enabled = false;

        float startTime = Time.timeScale;
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        // Enable UI interaction
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Slow down time
            Time.timeScale = Mathf.Lerp(startTime, 0f, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            // Fade UI in
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, t);

            yield return null;
        }

        Time.timeScale = 0f;
        // Restart game
    }
}
