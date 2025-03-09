using UnityEngine;

// This is all of the data that will be saved.
// Ensure objects are marked with [System.Serializable] attribute
// General settings are handled by GameState, so you shouldn't need to mess with them

[System.Serializable]
public class GameStateData
{
    [Space]
    [Header("General")]
    public string SaveId = GameState.DEFAULT_SAVE_ID;
    public string ApplicationVersion;
    public string SaveDateTime;

    [Space]
    [Header("GPS Example")]
    public ExampleGPSData ExampleGpsData;
}
