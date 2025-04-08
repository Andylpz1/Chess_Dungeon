using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNode : MonoBehaviour
{
    public int levelIndex; // 关卡索引，例如 1, 2, 3
    private static string gameSceneName = "GameScene"; // 目标游戏场景

    private void OnMouseDown()
    {
        Debug.Log($"🎯 选择关卡 {levelIndex}，即将进入 {gameSceneName}");

        // 存储关卡索引，并标记是从 LevelNode 进入
        PlayerPrefs.SetInt("SelectedLevel", levelIndex);
        PlayerPrefs.SetInt("IsLevelNode", 1);
        PlayerPrefs.Save(); // 确保数据存储

        // 加载 GameScene
        SceneManager.LoadScene(gameSceneName);
        if (SaveSystem.GameSaveExists())
        {
            GameData gameData = SaveSystem.LoadGame();
            GameManager.Instance.LoadGameData(gameData);
        }
        else
        {
            // 构造一个默认的 GameData 对象，确保必要字段有默认值
            //gameData = new GameData();
            // 例如：gameData.playerHealth = 默认血量; gameData.playerArmor = 默认护甲; 等
        }
    }

}
