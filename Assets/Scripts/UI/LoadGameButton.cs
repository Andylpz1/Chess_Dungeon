using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameButton : MonoBehaviour
{
    public void LoadGame()
    {
        // Check if a save file exists
        if (SaveSystem.GameSaveExists())
        {
            GameData gameData = SaveSystem.LoadGame();
            if (gameData != null)
            {
                
                // Load the saved level
                SceneManager.LoadScene("GameScene");

                // Optionally, pass saved data to the game manager or player
                GameManager.Instance.LoadGameData(gameData);


                Debug.Log("Loaded saved game successfully.");
            }
            else
            {
                Debug.LogError("Failed to load game data.");
            }
        }
        else
        {
            Debug.LogWarning("No save file found.");
        }
    }
}
