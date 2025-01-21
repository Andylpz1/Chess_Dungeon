using System.IO;
using UnityEngine;
using System.Collections; // Required for IEnumerator

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public MonsterManager monsterManager; 
    private void Awake()
    {
        if (monsterManager == null)
        {
            monsterManager = FindObjectOfType<MonsterManager>();
            if (monsterManager == null)
            {
                Debug.LogError("MonsterManager not found in the scene!");
            }
        }
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SaveGame()
    {
        GameData gameData = new GameData();

        // Check Player.Instance
        if (Player.Instance == null)
        {
            Debug.LogError("Player.Instance is null!");
            return;
        }

        // Save player health
        gameData.playerHealth = Player.Instance.health;

        // Check GetDeckNames
        if (Player.Instance.GetDeckNames() == null)
        {
            Debug.LogError("Player's deck is null or not initialized!");
            return;
        }

        // Save player deck
        gameData.playerDeck = Player.Instance.GetDeckNames();

        // Save current hand
        //gameData.playerHand = Player.Instance.deckManager.GetHandNames();

        // Check MonsterManager.Instance
        if (Player.Instance.monsterManager == null)
        {
            Debug.LogError("MonsterManager.Instance is null!");
            return;
        }

        // Save current level
        gameData.currentLevel = Player.Instance.monsterManager.GetCurrentLevel();

        // Write the save data
        SaveSystem.SaveGame(gameData);

        Debug.Log("Game saved successfully.");
    }


    public void LoadGameData(GameData gameData)
    {
        StartCoroutine(WaitForPlayerAndLoad(gameData));
    }

    private IEnumerator WaitForPlayerAndLoad(GameData gameData)
    {
        // 等待 Player 初始化
        while (Player.Instance == null)
        {
            Debug.LogWarning("等待 Player 初始化...");
            yield return null;
        }

        Debug.Log("Player 初始化完成，恢复玩家状态和加载关卡。");

        
        
        while (Player.Instance.monsterManager == null)
        {
            Debug.LogWarning("等待 MonsterManager 初始化...");
            yield return null;
        }
        // 确保 MonsterManager 存在 
        // 恢复玩家状态
        Player.Instance.SetHealth(gameData.playerHealth);
        Player.Instance.LoadDeck(gameData.playerDeck);
        //Player.Instance.deckManager.LoadHand(gameData.playerHand);
        //Player.Instance.monsterManager.StartLevel(gameData.currentLevel);
        

    }


    
}
