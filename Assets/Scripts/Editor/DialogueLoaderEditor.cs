using Codice.CM.Common;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DialogueLoaderEditor : EditorWindow
{

    [MenuItem("Tools/Load Dialogue CSV")]
    public static void LoadDialogueCSV()
    {
        string csvFiles = DialogueData.DIALOGUE_CSV_PATH;

        if (csvFiles.Length == 0)
        {
            Debug.LogError("No dialogue CSV files found in " + "Assets/Resources/" + DialogueData.DIALOGUE_CSV_PATH);
            return;
        }
        ConvertSingleCSV("Assets/Resources/" + csvFiles);


        //EditorGUIUtility.PingObject(assets[0]);

        AssetDatabase.Refresh(); // Refresh Unity assets
        Debug.Log("All dialogue CSVs converted to JSON successfully!");
    }

    private static void ConvertSingleCSV(string csvPath)
    {
        string fileName = Path.GetFileNameWithoutExtension(csvPath); 
        string jsonPath = Path.Combine("Assets/Resources/" + DialogueData.DIALOGUE_JSON_PATH + ".json");

        List<DialogueLine> dialogueList = new List<DialogueLine>();
        string[] lines = File.ReadAllLines(csvPath + ".csv");

        for (int i = 1; i < lines.Length; i++) // Skip the header row
        {
            string[] data = ParseCSVLine(lines[i]);

            //if (data.Length < 5) continue;

            dialogueList.Add(new DialogueLine
            {
                ID = int.Parse(data[0]),
                DialogueType = data[1].Trim(),
                Speaker = data[2].Trim(),
                Dialogue = data[3].Trim(),
                AfterIntervalGap = string.IsNullOrWhiteSpace(data[4]) ? 0f : float.Parse(data[4]),
                SpecialData = data.Length > 5 ? data[5].Trim() : ""
            });
        }

        string json = JsonUtility.ToJson(new DialogueData { dialogues = dialogueList }, true);
        File.WriteAllText(jsonPath, json);

        Debug.Log("Converted " + csvPath + " → " + jsonPath);
    }
    private static string[] ParseCSVLine(string line)
    {
        // Regex to split by ',' but ignore commas inside double quotes
        string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        string[] values = Regex.Split(line, pattern);

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i].StartsWith("\"") && values[i].EndsWith("\""))
            {
                values[i] = values[i].Substring(1, values[i].Length - 2); // Remove only the outermost quotes
            }

            values[i] = values[i].Replace("\"\"", "\""); // Convert "" to "
        }

        return values;
    }
}


