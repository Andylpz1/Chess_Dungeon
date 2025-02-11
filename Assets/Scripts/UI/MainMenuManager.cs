using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        ClearSaveData();
        // 加载游戏主场景，确保场景名称正确
        SceneManager.LoadScene("GameScene");
        MusicManager.Instance.PlayBackgroundMusic(MusicManager.Instance.battleMusic);
    }

    public void ExitGame()
    {
        // 退出游戏
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    private void ClearSaveData()
    {
        if (SaveSystem.SaveFileExists())
        {
            Debug.Log("清除存档...");
            SaveSystem.DeleteSaveFile(); // 调用 SaveSystem 的删除方法
        }
        else
        {
            Debug.Log("没有找到存档，无需清除。");
        }
    }
}
