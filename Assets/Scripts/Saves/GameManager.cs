using System.IO;
using UnityEngine;
using System.Collections; // Required for IEnumerator
using System.Collections.Generic;

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

    void Start()
    {
        if (SaveSystem.SaveFileExists())
        {
            GameData gameData = SaveSystem.LoadGame();
            LoadGameData(gameData);
        }   
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
        //洗回手牌
        Player.Instance.deckManager.RestartHand();

        // Save player health
        gameData.playerHealth = Player.Instance.health;

        // **存储当前卡组**
        gameData.playerDeckIds = new List<string>();
        foreach (var card in Player.Instance.deckManager.deck)
        {
            if (card != null) 
            {
                gameData.playerDeckIds.Add(card.Id);  // 存储卡牌 ID
            }
        }

        // **存储当前手牌**
        gameData.playerHandIds = new List<string>();
        foreach (var card in Player.Instance.deckManager.hand)
        {
            if (card != null)
            {
                gameData.playerHandIds.Add(card.Id);
            }
        }

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

        // **从 `Id` 重新创建 `Card` 并恢复 `deckManager.deck`**
        List<Card> restoredDeck = new List<Card>();
        foreach (string cardId in gameData.playerDeckIds)
        {
            Card restoredCard = CardDatabase.Instance.GetCardById(cardId);
            if (restoredCard != null)
            {
                restoredDeck.Add(restoredCard);
            }
            else
            {
                Debug.LogError($"Failed to load card with ID: {cardId}");
            }
        }
    
        // **恢复 `DeckManager` 的 `deck`**
        Player.Instance.deckManager.LoadDeck(restoredDeck);

        // **恢复手牌**
        List<Card> restoredHand = new List<Card>();
        foreach (string cardId in gameData.playerHandIds)
        {
            Card restoredCard = CardDatabase.Instance.GetCardById(cardId);
            if (restoredCard != null)
            {
                restoredHand.Add(restoredCard);
            }
            else
            {
                Debug.LogError($"CardDatabase 找不到 ID 为 {cardId} 的手牌！");
            }
        }
        //Player.Instance.deckManager.LoadHand(restoredHand); // 调用 DeckManager 处理手牌恢复

        //Player.Instance.deckManager.LoadHand(gameData.playerHand);
        //Player.Instance.monsterManager.StartLevel(gameData.currentLevel);
        

    }


    
}
