using System.Diagnostics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameState))]
public class GameStateEditor : Editor
{
    private string _saveId;
    private string _loadId;

    public override void OnInspectorGUI()
    {
        GameState gameState = (GameState)target;

        GUILayout.Space(15f);
        GUILayout.Label("Quick", EditorStyles.boldLabel);

        if(GUILayout.Button("Quick Save"))
        {
            gameState.QuickSave();
        }

        if (GUILayout.Button("Quick Load"))
        {
            gameState.QuickLoad();
        }

        GUILayout.Space(15f);
        GUILayout.Label("File", EditorStyles.boldLabel);

        if (GUILayout.Button("Save to File"))
        {
            string filePath = EditorUtility.SaveFilePanel("Create Save File", GameState.GetSaveFolderPath(), gameState.Data.SaveId, "json");
            if (filePath != string.Empty)
            {
                gameState.SaveToFile(filePath);
            }

        }

        if (GUILayout.Button("Load from File"))
        {
            string filePath = EditorUtility.OpenFilePanel("Select Save File", GameState.GetSaveFolderPath(), "json");
            if (filePath != string.Empty)
            {
                gameState.LoadFromFile(filePath);
            }
        }

        GUILayout.Space(15f);
        GUILayout.Label("Id", EditorStyles.boldLabel);

        // Save to id button
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save to Id"))
        {
            gameState.SaveToId(_saveId);
        }
        _saveId = GUILayout.TextField(_saveId, GUILayout.MaxWidth(300));

        GUILayout.EndHorizontal();

        // Load from id button
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Load from Id"))
        {
            gameState.LoadFromId(_loadId);
        }
        _loadId = GUILayout.TextField(_loadId, GUILayout.MaxWidth(300));

        GUILayout.EndHorizontal();

        GUILayout.Space(15f);
        GUILayout.Label("Debugging", EditorStyles.boldLabel);

        // Open save file folder button
        if (GUILayout.Button("Open Saves Directory"))
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = $"{GameState.GetSaveFolderPath()}",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        // Debug list all ids button
        if (GUILayout.Button("Debug List All Save Ids"))
        {
            foreach (string saveId in GameState.GetAllSaveIds())
            {
                UnityEngine.Debug.Log($"Save Id: {saveId}");
            }
        }

        GUILayout.Space(20f);
        base.OnInspectorGUI();
    }
}
