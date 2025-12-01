using DG.Tweening;
using Kudoshi.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SanityManager : Singleton<SanityManager>
{
    [Header("Settings")]
    public Image sanityImg; 
    public float currentSanity;
    public float maxSanity = 100f;
    public float slowDuration;
    public float overtimeDecreaseRate = 3f;
    public float _hitKnockbackForce;
    public CanvasGroup canvasGroup;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentSanity = maxSanity;
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DecreaseSanity(10f, Vector3.zero);
        }

        InternalSanityUpdate();
    }

    private void InternalSanityUpdate()
    {
        currentSanity -= overtimeDecreaseRate * Time.deltaTime;

        //if (currentSanity <= 200)
        //{
        //    SoundManager.Instance.PlaySound("sfx_lowhealth_heartbeat");
        //}

        sanityImg.fillAmount = Mathf.Lerp(sanityImg.fillAmount, currentSanity / maxSanity, Time.deltaTime * 10f);

        if (currentSanity <= 0) GameOver();
    }

    private void UpdateSanity(float current_sanity, float max_sanity)
    {
        currentSanity = current_sanity;
        maxSanity = max_sanity;
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

    public void DecreaseSanity(float sanity, Vector3 knockbackDirection)
    {
        currentSanity -= sanity;

        if (currentSanity <= 0)
        {
            currentSanity = 0;
            GameOver();

        }

        _rb.AddForce(knockbackDirection * _hitKnockbackForce, ForceMode.Impulse);

        // Add subtle camera shake when player gets hit
        if (PlayerCamera.Instance != null)
        {
            PlayerCamera.Instance.Shake(0.15f, 0.15f);
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

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Restart game
    }
}
