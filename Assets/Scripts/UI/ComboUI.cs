using UnityEngine;
using TMPro;
using DG.Tweening;

public class ComboUI : MonoBehaviour
{
    public static ComboUI Instance;

    public TextMeshProUGUI comboText;
    public CanvasGroup canvasGroup;

    private int killScore = 10;

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

    public void AddKill()
    {
        // Show score
        comboText.text = $"x{killScore}!";

        // Random pastel color
        comboText.color = pastelColors[Random.Range(0, pastelColors.Length)];

        // Show and pop
        canvasGroup.alpha = 1f;
        comboText.transform.localScale = Vector3.one;
        comboText.transform.DOPunchScale(Vector3.one * 0.5f, 0.3f).SetEase(Ease.OutBack);

        // Fade out
        canvasGroup.DOFade(0f, 0.5f).SetDelay(0.5f);

        // Multiply for next kill
        killScore *= 10;
    }
}
