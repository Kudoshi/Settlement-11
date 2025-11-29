using UnityEngine;
using Kudoshi.Utilities;
using System;
using UnityEngine.UI;

public class SanityManager : Singleton<SanityManager>
{
    [Header("Settings")]
    public Image sanityImg; 
    public float currentSanity;
    public float maxSanity;

    private void Start()
    {
        maxSanity = 100f;
        currentSanity = maxSanity;
    }

    // Update is called once per frame
    void Update()
    {
        InternalSanityUpdate();
    }

    private void InternalSanityUpdate()
    {
        currentSanity -= 5f * Time.deltaTime;
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
        UpdateSanity(currentSanity, maxSanity);
    }

    public void DecreaseSanity(float sanity)
    {
        currentSanity -= sanity;
        UpdateSanity(currentSanity, maxSanity);
    }
}
