using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveAndExitButton : MonoBehaviour
{
    public GameManager gameManager; // Reference to GameManager
    public DeckManager deckManager;

    private void Awake()
    {
        // Automatically find GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        deckManager = FindObjectOfType<DeckManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }
    public void SaveAndExitToMenu()
    {
        // Save the game data
        if (gameManager != null)
        {
            deckManager.RestoreExhaustedCards();
            gameManager.SaveGame();
            Debug.Log("Game data saved.");
        }
        else
        {
            Debug.LogError("GameManager not assigned!");
        }

        // Load the main menu scene
        SceneManager.LoadScene("MainMenu"); // Replace "MainMenu" with your main menu scene name
    }
}
