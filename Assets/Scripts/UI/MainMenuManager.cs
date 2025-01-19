using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // 加载游戏主场景，确保场景名称正确
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        // 退出游戏
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
