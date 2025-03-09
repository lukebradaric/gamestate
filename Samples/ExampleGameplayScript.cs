using UnityEngine;

public class ExampleGameplayScript : MonoBehaviour
{
    // Reference to the GameState
    public GameState _gameState;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Generate random number and random location
            int rand = Random.Range(0, 3);
            string location = string.Empty;
            switch (rand)
            {
                case 0:
                    location = "House";
                    break;
                case 1:
                    location = "Gas Station";
                    break;
                case 2:
                    location = "Bus Stop";
                    break;
            }

            // Directly set values in the game state
            // These values will automatically be saved/loaded when calling gamestate.save()/.load()
            _gameState.Data.ExampleGpsData.ExampleLastVisitedLocation = location;
            _gameState.Data.ExampleGpsData.LastPassengerId = rand;
        }
    }
}
