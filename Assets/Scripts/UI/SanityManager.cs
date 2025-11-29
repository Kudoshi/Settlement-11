using UnityEngine;
using Kudoshi.Utilities;
using System;
using UnityEngine.UI;

public class SanityManager : Singleton<SanityManager>
{
    [Header("Settings")]
    public Image sanityImg; 
    public float currentSanity;
    public float maxSanity = 100f;

    private void Start()
    {
        currentSanity = maxSanity;
    }

    // Update is called once per frame
    void Update()
    {
        InternalSanityUpdate();
    }

    private void InternalSanityUpdate()
    {
        currentSanity -= 3f * Time.deltaTime;
        sanityImg.fillAmount = Mathf.Lerp(sanityImg.fillAmount, currentSanity / maxSanity, Time.deltaTime * 10f);
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

    public void DecreaseSanity(float sanity)
    {
        currentSanity -= sanity;

        if (currentSanity < 0)
        {
            currentSanity = 0;
            GameOver();

        }

        UpdateSanity(currentSanity, maxSanity);
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
