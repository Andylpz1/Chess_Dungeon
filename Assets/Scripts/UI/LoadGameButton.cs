using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGameButton : MonoBehaviour
{
    public void LoadGame()
    {
        if (!SaveSystem.GameSaveExists())
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        GameData gameData = SaveSystem.LoadGame();
        if (gameData == null)
        {
            Debug.LogError("Failed to load game data.");
            return;
        }

        // 清除 IsLevelNode，确保不当作关卡节点入口
        PlayerPrefs.SetInt("IsLevelNode", 0);

        // 读取并清除“回到选关”标记
        int returnToLS = PlayerPrefs.GetInt("ReturnToLevelSelection", 0);
        PlayerPrefs.SetInt("ReturnToLevelSelection", 0);
        PlayerPrefs.Save();

        if (returnToLS == 1)
        {
            // 回到选关界面
            SceneManager.LoadScene("LevelSelectionScene");
            Debug.Log("🔙 Returning to Level Selection");
        }
        else
        {
            // 正常恢复到游戏场景
            SceneManager.LoadScene("GameScene");
            GameManager.Instance.LoadGameData(gameData);
            Debug.Log("▶️ Loaded saved game successfully.");
        }
    }
}
