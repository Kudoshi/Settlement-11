using UnityEngine;
using TMPro;
using DG.Tweening;

public class ComboUI : MonoBehaviour
{
    public static ComboUI Instance;

    public TextMeshProUGUI comboText;
    public CanvasGroup canvasGroup;

    public float comboResetTime = 2f;

    private int killScore = 10;
    private float comboTimer = 0f;

    private Color[] pastelColors = new Color[]
    {
        new Color(1f, 0.7f, 0.7f), // Pastel red
        new Color(1f, 0.9f, 0.6f), // Pastel yellow
        new Color(0.7f, 1f, 0.7f), // Pastel green
        new Color(0.6f, 0.9f, 1f), // Pastel blue
        new Color(1f, 0.7f, 1f),   // Pastel pink
        new Color(1f, 0.6f, 0.4f)  // Pastel orange
    };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    public void AddKill()
    {
        // Reset timer
        comboTimer = comboResetTime;

        // Show score
        comboText.text = $"x{killScore}!";

        // Random pastel color
        comboText.color = pastelColors[Random.Range(0, pastelColors.Length)];

        // Kill previous animations
        comboText.transform.DOKill();
        canvasGroup.DOKill();

        // Show and JUICY pop
        canvasGroup.alpha = 1f;
        comboText.transform.localScale = Vector3.one;

        // Big punch scale
        comboText.transform.DOPunchScale(Vector3.one * 0.8f, 0.4f, 8, 0.5f).SetEase(Ease.OutElastic);

        // Shake rotation for extra juice
        comboText.transform.DOShakeRotation(0.3f, new Vector3(0, 0, 20f), 10, 90f);

        // Camera shake gets stronger with multiplier
        if (PlayerCamera.Instance != null)
        {
            float intensity = Mathf.Min(0.15f * Mathf.Log10(killScore), 0.4f);
            PlayerCamera.Instance.Shake(intensity, 0.15f);
        }

        // Fade out
        canvasGroup.DOFade(0f, 0.5f).SetDelay(0.8f);

        // Multiply for next kill
        killScore *= 10;
    }

    private void ResetCombo()
    {
        killScore = 10;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.3f);
        Debug.Log("Combo reset!");
    }
}
