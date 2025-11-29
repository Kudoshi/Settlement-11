using UnityEngine;
using TMPro;
using System.Collections;

public class FireflyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text spinText;
    [SerializeField] private TMP_Text dashText;

    private int spinCost = 25;
    private int dashCost = 10;

    private void Start()
    {
        PlayerFirefly.Instance.OnFireflyChanged += UpdateUI;
        UpdateUI();
    }

    private void UpdateUI()
    {
        int current = PlayerFirefly.Instance.FireflyCounter;

        float spinTarget = Mathf.Clamp01((float)current / spinCost) * 100f;
        float dashTarget = Mathf.Clamp01((float)current / dashCost) * 100f;

        StartCoroutine(CountTo(spinText, spinTarget));
        StartCoroutine(CountTo(dashText, dashTarget));
    }

    private IEnumerator CountTo(TMP_Text textObj, float target)
    {
        float current = 0f;
        float speed = 50f;

        while (current < target)
        {
            current += speed * Time.deltaTime;
            if (current > target) current = target;
            textObj.text = $"{current:F0}%";
            yield return null;
        }
    }
}
