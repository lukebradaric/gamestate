using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExampleGameStateUI : MonoBehaviour
{
    public GameState _gameState;

    public Transform _verticalLayoutTransform;
    public GameObject _saveLoadButtonPrefab;

    public async void Start()
    {
        foreach (GameStateData gameStateData in await GameState.GetAllSaveData())
        {
            GameObject saveLoadButton = Instantiate(_saveLoadButtonPrefab, _verticalLayoutTransform);
            saveLoadButton.GetComponentInChildren<TextMeshProUGUI>().text = $"{gameStateData.SaveId} ({gameStateData.SaveDateTime})";
            saveLoadButton.GetComponent<Button>().onClick.AddListener(delegate { OnClickListener(gameStateData); });
        }
    }

    private void OnClickListener(GameStateData gameStateData)
    {
        Debug.Log($"Loading GameStateData: {gameStateData.SaveId}");
        _gameState.Load(gameStateData);
    }
}
