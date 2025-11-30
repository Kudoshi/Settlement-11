using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class DialogueTypewriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private string[] dialogueLines;
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
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

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
                yield return new WaitForSeconds(delayBetweenLines);
                dialogueText.DOColor(lastLineColor, colorChangeDuration);
                yield return new WaitForSeconds(colorChangeDuration + 1f);
            }
            else
            {
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }

        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, fadeDuration);
        }
    }
}
