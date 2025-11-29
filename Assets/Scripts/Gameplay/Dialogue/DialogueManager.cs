
using Kudoshi.Utilities;
using System.Collections;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueData _dialogueData;
    private int _currentDialogueIdx = 0;
    private string _dialogueAudioPrefix;
    private bool _isLevelDialogueAvailable;
    private Coroutine _dialogueCr;
    private bool _subtitleEnabled = true;

    public int CurrentDialogueIdx => _currentDialogueIdx;

    public DialogueLine CurrentDialogueLine => _dialogueData.dialogues[_currentDialogueIdx];

    public DialogueData DialogueData => _dialogueData;

    public bool IsLevelDialogueAvailable => _isLevelDialogueAvailable;

    private void Start()
    {
        LoadDialogueData();
        FetchSubtitleSettings();

        _currentDialogueIdx = 0;
    }

    public void LoadDialogueData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(DialogueData.DIALOGUE_JSON_PATH);
        _dialogueAudioPrefix = "voice_";

        if (jsonFile == null)
        {
            _isLevelDialogueAvailable = false;
            Debug.LogWarning("Dialogue JSON file not found: " + DialogueData.DIALOGUE_JSON_PATH);
            return;
        }
        else
        {
            _dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text);
            Debug.Log("[DialogueManager] Dialogue JSON file loaded: " + DialogueData.DIALOGUE_JSON_PATH);
            _isLevelDialogueAvailable = true;
            SoundManager.Instance.LoadDialogueVoicelines();

            //foreach (var dialogue in _dialogueData.dialogues)
            //{
            //    Debug.Log($"[{dialogue.Speaker}] {dialogue.Dialogue} (Type: {dialogue.DialogueType}, Delay: {dialogue.AfterIntervalGap}s)");
            //}
        }
    }


    public void PlayDialogueID(int idx)
    {
        _currentDialogueIdx = idx;

        if (_dialogueCr != null)
            StopCoroutine(_dialogueCr);

        if (_isLevelDialogueAvailable)
            _dialogueCr = StartCoroutine(StartDialogue(_currentDialogueIdx));
    }

    public void FetchSubtitleSettings()
    {
        _subtitleEnabled = PlayerPrefs.GetInt(GlobalSettings.KEY_SETTING_SUBTITLE, 1) == 1 ? true : false;
    }

    private IEnumerator StartDialogue(int currentDialogueIdx)
    {
        while(_currentDialogueIdx < _dialogueData.dialogues.Count)
        {
            DialogueLine line = _dialogueData.dialogues[_currentDialogueIdx];
            bool noSubtitle = false;
            bool lastLine = false;

            if (line.DialogueType.Contains(DialogueData.KEY_CONVERSATION_END))
            {
                lastLine = true;
            }

            // Check special data
            if (line.SpecialData.Length != 0)
            {
                if (line.SpecialData.Contains(DialogueData.KEY_CONVERSATION_SKIP))
                {
                    _currentDialogueIdx++;
                    if (lastLine) break;
                    else continue;
                }
                else if (line.SpecialData.Contains(DialogueData.KEY_CONVERSATION_NO_SUBTITLE))
                {
                    noSubtitle = true;
                }
            }

            // Play voice lines
            float dialogueLength = 2f;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayDialogue(_dialogueAudioPrefix + _currentDialogueIdx);
                AudioClip audioClip = SoundManager.Instance.GetCurrentDialogueClip();
                dialogueLength = audioClip.length;
            }

            // Set UI subtitles
            if (_subtitleEnabled && !noSubtitle)
            {
                DialogueUI.Instance.SetDialogueText(line, dialogueLength);
            }

            if (line.DialogueType.Contains(DialogueData.KEY_CONVERSATION_END))
            {
                break;
            }

            _currentDialogueIdx++;

            yield return new WaitForSeconds(dialogueLength + line.AfterIntervalGap);
        }

        _dialogueCr = null;
    }

    public void StopDialogue()
    {
        if (_dialogueCr != null)
        {
            StopCoroutine(_dialogueCr);
            SoundManager.Instance.StopDialogue();
            DialogueUI.Instance.StopDialogue();

            _dialogueCr = null;
        }
    }
}