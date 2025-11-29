
using log4net.Core;
using System;
using UnityEditor;
using UnityEngine;

public class DialogueTesterEditor : EditorWindow
{
    private int _dialogueID = 0;

    [MenuItem("Tools/Dialogue Tester")]
    public static void ShowWindow()
    {
        GetWindow<DialogueTesterEditor>("Dialogue Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Dialogue Manager", EditorStyles.boldLabel);
        GUILayout.Label("---------------------");
        GUILayout.Label("* Tool only works during play mode");
        GUILayout.Space(5);

        // Input for Dialogue ID
        _dialogueID = EditorGUILayout.IntField("Dialogue ID", _dialogueID);

        GUILayout.Space(10);

        // Button to Play Dialogue
        if (GUILayout.Button("Play Dialogue"))
        {
            PlayDialogue(_dialogueID);
        }

        if (GUILayout.Button("Stop Dialogue"))
        {
            StopDialogue();
        }
    }

    private void PlayDialogue(int dialogueID)
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("[DialogueTesterEditor] Tool only works during play mode");
            return;
        }

        DialogueManager.Instance.PlayDialogueID(dialogueID);
    }

    private void StopDialogue()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("[DialogueTesterEditor] Tool only works during play mode");
            return;
        }

        DialogueManager.Instance.StopDialogue();

    }

}