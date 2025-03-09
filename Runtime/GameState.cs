using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

// Commented this out because you shouldn't have to create more than 1 GameState ScriptableObject
//[CreateAssetMenu(menuName = "GameState")]
public class GameState : ScriptableObject
{
    public const string DEFAULT_SAVE_ID = "default";

    [SerializeField] private GameStateData _data;
    public GameStateData Data => _data;

    public event Action OnBeforeGameStateSaved;
    public event Action OnAfterGameStateSaved;
    public event Action OnBeforeGameStateLoaded;
    public event Action OnAfterGameStateLoaded;

    /// <summary>
    /// Saves current GameStateData to Default Save Id.
    /// </summary>
    /// <returns></returns>
    public async Task QuickSave()
    {
        await SaveToId(DEFAULT_SAVE_ID);
    }

    /// <summary>
    /// Saves current GameStateData to a file based on Save Id.
    /// </summary>
    /// <param name="saveId">The Id of the save file.</param>
    public async Task SaveToId(string saveId)
    {
        OnBeforeGameStateSaved?.Invoke();

        // Set general save/load variables
        Data.ApplicationVersion = Application.version;
        Data.SaveDateTime = String.Format("{0:F}", DateTime.Now.ToString());

        // Ensure saveId is valid (Not empty, no special characters, no spaces)
        saveId = SanitizeSaveId(saveId);
        Data.SaveId = saveId;

        await SaveToFile(GetSaveFilePath(saveId));
    }

    /// <summary>
    /// Saves current GameStateData to a specific file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task SaveToFile(string filePath)
    {
        // Try to save data to json file
        try
        {
            // Serialize this object to JSON
            string jsonString = JsonUtility.ToJson(_data, true);

            // Save to file path (save name or save id)
            await File.WriteAllTextAsync(filePath, jsonString);
        }
        catch (Exception exception)
        {
            Debug.LogError($"GameState: Error saving to file! ({exception.Message})");
        }

        OnAfterGameStateSaved?.Invoke();
    }

    /// <summary>
    /// Loads GameStateData from Default Save Id.
    /// </summary>
    /// <returns></returns>
    public async Task QuickLoad()
    {
        await LoadFromId(DEFAULT_SAVE_ID);
    }

    /// <summary>
    /// Loads GameStateData from a Save Id.
    /// </summary>
    /// <param name="saveId">The Id of the save file.</param>
    public async Task LoadFromId(string saveId = DEFAULT_SAVE_ID)
    {
        OnBeforeGameStateLoaded?.Invoke();

        // Ensure saveId is valid (Not empty, no special characters, no spaces)
        saveId = SanitizeSaveId(saveId);

        await LoadFromFile(GetSaveFilePath(saveId));
    }

    /// <summary>
    /// Loads GameStateData from a specific file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task LoadFromFile(string filePath)
    {
        try
        {
            // Load text from save file path
            string jsonString = await File.ReadAllTextAsync(filePath);

            // Load GameStateData from json string
            GameStateData gameStateData = JsonUtility.FromJson<GameStateData>(jsonString);

            // Load data
            Load(gameStateData);
        }
        catch (Exception exception)
        {
            Debug.LogError($"GameState: Error loading save file! ({exception.Message})");
        }
    }

    /// <summary>
    /// Loads GameStateData directly from GameStateData object.
    /// </summary>
    /// <param name="gameStateData"></param>
    public void Load(GameStateData gameStateData)
    {
        this._data = gameStateData;

        OnAfterGameStateLoaded?.Invoke();
    }

    /// <summary>
    /// Returns the path of the save file root folder.
    /// </summary>
    /// <returns></returns>
    public static string GetSaveFolderPath()
    {
        // If folder doesn't exist, create saves folder directorty
        string folderPath = $"{Application.persistentDataPath}\\saves";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return folderPath;
    }

    /// <summary>
    /// Returns the path of a save file based on Id.
    /// If the file doesn't exist, it will be created.
    /// </summary>
    /// <param name="saveId"></param>
    /// <returns></returns>
    public static string GetSaveFilePath(string saveId = DEFAULT_SAVE_ID)
    {
        string filePath = $"{GetSaveFolderPath()}\\{saveId}.json";

        // If file doesn't exist yet, create new file
        if (!File.Exists(filePath))
        {
            FileStream stream = File.Create(filePath);
            // Dispose of stream so file is unlocked for reading/writing
            stream.Dispose();
        }

        return filePath;
    }

    /// <summary>
    /// Returns a collection of all Save Id's within the saves folder.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetAllSaveIds()
    {
        HashSet<string> ids = new HashSet<string>();

        DirectoryInfo directoryInfo = new DirectoryInfo(GetSaveFolderPath());

        foreach (var file in directoryInfo.GetFiles("*.json"))
        {
            ids.Add(ParseSaveIdFromFileName(file.Name));
        }

        return ids;
    }

    /// <summary>
    /// Returns a collection of all GameStateData from all save files in the save folder.
    /// WARNING: Depending on GameStateData object size, this can be a costly call.
    /// It may be best to call this on game start, and cache the result for later use.
    /// </summary>
    /// <returns></returns>
    public static async Task<IEnumerable<GameStateData>> GetAllSaveData()
    {
        HashSet<GameStateData> data = new HashSet<GameStateData>();

        DirectoryInfo directoryInfo = new DirectoryInfo(GetSaveFolderPath());

        foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.json"))
        {
            string jsonString = await File.ReadAllTextAsync(fileInfo.FullName);
            GameStateData gameStateData = JsonUtility.FromJson<GameStateData>(jsonString);
            data.Add(gameStateData);
        }

        return data;
    }

    // Parses SaveId from a file name.
    // We could deserialize the entire file and reach the _saveId string but,
    // this will save performance in the long run, especially if we have
    // massive save files.
    private static string ParseSaveIdFromFileName(string fileName)
    {
        return fileName.Replace(".json", "");
    }

    // Removes any special characters, and replaces spaces with underscores
    // If saveId is empty, replaces with default_save_id
    // Create a proper saveId string for file naming
    private static string SanitizeSaveId(string saveId)
    {
        string temp = saveId;

        // If saveId is empty, return default saveId
        if (saveId == string.Empty)
        {
            return DEFAULT_SAVE_ID;
        }

        // Remove all special characters
        saveId = Regex.Replace(saveId, "[^a-zA-Z0-9_ ]+", "", RegexOptions.Compiled);
        // Replace multiple spaces with single space
        saveId = Regex.Replace(saveId, @"\s+", " ").Trim();
        // Replace spaces with underscores
        saveId = saveId.Replace(" ", "_");

        if (temp != saveId)
        {
            Debug.LogWarning($"GameState: Invalid Save Id converted. ({temp}) > ({saveId})");
        }

        return saveId;
    }
}
