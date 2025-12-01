using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
    public Light[] trafficLights = new Light[3]; // 0: Red, 1: Yellow, 2: Green
    public float lightDuration = 3f;
    public float flickerDuration = 0.0f; // Set to 0 if no flicker before change, otherwise e.g. 0.5f

    private int _currentIndex = 0;

    void Start()
    {
        TurnAllLightsOff();
        StartCoroutine(TrafficLightCycle());
    }

    void TurnAllLightsOff()
    {
        foreach (Light light in trafficLights)
        {
            if (light != null) light.enabled = false;
        }
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // Turn on current light
            if (trafficLights[_currentIndex] != null)
            {
                trafficLights[_currentIndex].enabled = true;
            }

            yield return new WaitForSeconds(lightDuration);

            // Start flickering if flickerDuration is greater than 0
            if (flickerDuration > 0)
            {
                yield return StartCoroutine(FlickerLight(trafficLights[_currentIndex]));
            }

            // Turn off current light
            if (trafficLights[_currentIndex] != null)
            {
                trafficLights[_currentIndex].enabled = false;
            }

            _currentIndex = (_currentIndex + 1) % trafficLights.Length;
        }
    }

    IEnumerator FlickerLight(Light lightToFlicker)
    {
        if (lightToFlicker == null) yield break;

        float timer = 0f;
        while (timer < flickerDuration)
        {
            lightToFlicker.enabled = !lightToFlicker.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        lightToFlicker.enabled = false; // Ensure it's off after flickering
    }
}