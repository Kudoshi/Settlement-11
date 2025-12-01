using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

public class DialogueTypewriter : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private string[] dialogueLines;

    [Header("Image (Optional)")]
    [SerializeField] private RawImage dialogueImage;
    [SerializeField] private CanvasGroup imageCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float delayBetweenLines = 1f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color lastLineColor = Color.red;
    [SerializeField] private float colorChangeDuration = 0.5f;

    private void Start()
    {
        if (dialogueText != null && dialogueLines.Length > 0)
        {
            StartCoroutine(ShowDialogue());
        }
    }

    private IEnumerator ShowDialogue()
    {
        // Show text canvas
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        // Show image if assigned
        if (imageCanvasGroup != null)
        {
            imageCanvasGroup.alpha = 0f;
            imageCanvasGroup.DOFade(1f, fadeDuration);
        }
        else if (dialogueImage != null)
        {
            // If no canvas group but image exists, just show it
            Color c = dialogueImage.color;
            c.a = 1f;
            dialogueImage.color = c;
        }

        // Type out dialogue lines
        for (int i = 0; i < dialogueLines.Length; i++)
        {
            string line = dialogueLines[i];
            dialogueText.text = "";

            foreach (char c in line)
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }

            if (i == dialogueLines.Length - 1)
            {
                // Last line - change color
                yield return new WaitForSeconds(delayBetweenLines);
                dialogueText.DOColor(lastLineColor, colorChangeDuration);
                yield return new WaitForSeconds(colorChangeDuration + 1f);
            }
            else
            {
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }

        // Fade out everything
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, fadeDuration);
        }

        if (imageCanvasGroup != null)
        {
            imageCanvasGroup.DOFade(0f, fadeDuration);
        }
        else if (dialogueImage != null)
        {
            dialogueImage.DOFade(0f, fadeDuration);
        }
    }
}
