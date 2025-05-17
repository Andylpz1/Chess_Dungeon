using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionExitButton : MonoBehaviour
{
    private DeckManager deckManager;

    private void Awake()
    {
        deckManager = FindObjectOfType<DeckManager>();
    }

    public void OnExitButtonClicked()
    {
        // 跳回主菜单
        SceneManager.LoadScene("MainMenu");
    }
}
