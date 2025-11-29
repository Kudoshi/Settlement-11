using System.Collections.Generic;

[System.Serializable]
public class DialogueLine
{
    public int ID;
    public string DialogueType;
    public string Speaker;
    public string Dialogue;
    public float AfterIntervalGap;
    public string SpecialData;
}

[System.Serializable]
public class DialogueData
{
    public List<DialogueLine> dialogues;

    public const string DIALOGUE_CSV_PATH = "Dialogues/CSV/Dialogue";
    public const string DIALOGUE_JSON_PATH = "Dialogues/JSON/Dialogue";
    public const string KEY_CONVERSATION_END = "CONVERSATION_END";
    public const string KEY_CONVERSATION_SKIP = "SKIP";
    public const string KEY_CONVERSATION_NO_SUBTITLE = "NO_SUBTITLE";
}