using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1.2f;
    [SerializeField] private float flickerSpeed = 0.1f;
    [SerializeField] private float flickerAmount = 0.2f;

    private float baseIntensity;
    private float flickerTimer;
    private float nextFlickerTime;

    private void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        if (targetLight != null)
        {
            baseIntensity = targetLight.intensity;
        }

        nextFlickerTime = Random.Range(0.05f, 0.2f);
    }

    private void Update()
    {
        if (targetLight == null) return;

        flickerTimer += Time.deltaTime;

        if (flickerTimer >= nextFlickerTime)
        {
            float randomFlicker = Random.Range(-flickerAmount, flickerAmount);
            float targetIntensity = Mathf.Clamp(baseIntensity + randomFlicker, minIntensity, maxIntensity);
            targetLight.intensity = targetIntensity;

            flickerTimer = 0f;
            nextFlickerTime = Random.Range(0.05f, 0.2f);
        }
    }
}
