using System.IO;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public void SaveGame()
    {
        GameData gameData = new GameData();

        // 保存玩家血量
        gameData.playerHealth = Player.Instance.health;

        // 保存玩家卡组
        gameData.playerDeck = Player.Instance.GetDeckNames(); // 假设返回卡牌名称的列表

        // 保存关卡信息
        //gameData.currentLevel = LevelManager.Instance.GetCurrentLevel(); // 获取当前关卡编号

        // 写入存档文件
        SaveSystem.SaveGame(gameData);
    }

    public void LoadGame()
    {
        GameData gameData = SaveSystem.LoadGame();
        if (gameData == null) return;

        // 恢复玩家状态
        Player.Instance.SetHealth(gameData.playerHealth);
        Player.Instance.LoadDeck(gameData.playerDeck); // 加载卡组

        // 设置关卡
        //LevelManager.Instance.LoadLevel(gameData.currentLevel);

        Debug.Log("Game loaded successfully.");
    }
}
