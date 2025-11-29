
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Kudoshi.Utilities;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : Singleton<DialogueUI>
{
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private Image _dialogueBar;
    [SerializeField] private TextMeshProUGUI _dialogueText;

    [Header("Dialogue Settings")]
    [SerializeField] private SerializedDictionary<string, Color> _speakerColor;
    [SerializeField] private float _dialogueFadeInDuration = 0.15f;
    [SerializeField] private float _dialogueFadeOutDuration = 0.3f;
    [SerializeField] private float _dialogueEndWaitToFade = 0.15f;

    private Coroutine _fadeOutCr;

    // Appearance Variables
    private Color _dialogueBarColor;

    private void Start()
    {
        //Debug.Log("[DialogueUI] Start() called. Dialogue UI initialized.");

        // Ensure variables are resetted properly
        _dialoguePanel.SetActive(true);
        _dialogueBarColor = _dialogueBar.color;
        _dialogueBar.color = new Color(0,0,0,0);
        _dialogueText.color = new Color(1, 1, 1, 0);

    }

    public void SetDialogueText(DialogueLine line, float time)
    {
        DOTween.Kill(_dialogueBar);
        DOTween.Kill(_dialogueText);

        string dialogueText = line.DialogueType.Contains("SPEAKER") ? line.Speaker + ": " + line.Dialogue : line.Dialogue;
        Color speakerColor = _speakerColor.ContainsKey(line.Speaker)? _speakerColor[line.Speaker] : Color.white;
        
        _dialogueText.text = dialogueText;
        _dialogueText.color = speakerColor;

        if (_dialogueBar.color.a != 1)
        {
            speakerColor.a = 0;

            _dialogueBar.DOFade(_dialogueBarColor.a, _dialogueFadeInDuration);
            _dialogueText.DOFade(1f, _dialogueFadeInDuration);
        }

        // Wait for 'time' seconds, then fade out
        float timeWaitToFade = time + _dialogueEndWaitToFade;
        
        if (_fadeOutCr != null)
        {
            StopCoroutine(_fadeOutCr);
        }

        _fadeOutCr = StartCoroutine(FadeOutDialogueDisplay(timeWaitToFade));

    }

    public void StopDialogue()
    {
        DOTween.Kill(_dialogueBar);
        DOTween.Kill(_dialogueText);

        Color dialogueBar = _dialogueBar.color;
        Color dialogueText = _dialogueText.color;

        dialogueBar.a = 0;
        dialogueText.a = 0;

        _dialogueBar.color = dialogueBar;
        _dialogueText.color = dialogueText;
    }

    private IEnumerator FadeOutDialogueDisplay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _dialogueBar.DOFade(0f, _dialogueFadeOutDuration);
        _dialogueText.DOFade(0f, _dialogueFadeOutDuration);
        _fadeOutCr = null;
    }
}