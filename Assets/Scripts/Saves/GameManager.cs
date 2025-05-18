using System.IO;
using UnityEngine;
using System.Collections; // Required for IEnumerator
using System.Collections.Generic;
using UnityEngine.SceneManagement;  

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public MonsterManager monsterManager; 
    public List<Card> playerDeck = new List<Card>();
    public GameData currentGameData;
    public Player player;
    private void Awake()
    {
        if (monsterManager == null)
        {
            monsterManager = FindObjectOfType<MonsterManager>();
            if (monsterManager == null)
            {
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
    // 订阅／退订事件就写在类里
    void OnEnable() {
    SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable() {
    SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ↓ 这一定要是 private (或 public) void，接收两个参数，顺序和类型都要对
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
                           UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        player = FindObjectOfType<Player>();
    }


    void Start()
    {
        if (SaveSystem.GameSaveExists())
        {
            GameData gameData = SaveSystem.LoadGame();
            currentGameData = gameData;
            LoadGameData(gameData);
        }   
    }
    
    public void SaveGame()
    {
        GameData gameData = new GameData();

        //shuffle hand back
        Player.Instance.deckManager.RestartHand();

        // Save player health and armor
        gameData.playerHealth = Player.Instance.health;
        gameData.playerArmor = Player.Instance.armor;
        // Save current deck
        gameData.playerDeckIds = new List<string>();
        foreach (var card in Player.Instance.deckManager.deck)
        {
            //不储存临时卡
            if (card != null && !card.isTemporary) 
            {
                gameData.playerDeckIds.Add(card.Id);  // 存储卡牌 ID
            }
        }
        playerDeck = new List<Card>(Player.Instance.deckManager.deck);
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
    public void SaveDeck()
    {
        GameData currentGameData = SaveSystem.GameSaveExists()
                   ? SaveSystem.LoadGame()
                   : new GameData();
        if (currentGameData == null)
        {
            Debug.LogError("当前存档数据为空，无法保存牌组！");
            return;
        }

        List<string> deckIds = new List<string>();
        foreach (Card card in playerDeck)
        {
            if (card != null)
            {
                Debug.Log($"SaveDeck 写入 Id = {card.Id}");
                deckIds.Add(card.Id);
            }
        }
        currentGameData.playerDeckIds = deckIds;

        // 调用存档系统保存当前存档数据
        SaveSystem.SaveGame(currentGameData);
        Debug.Log("Deck saved successfully.");
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
        Player.Instance.SetArmor(gameData.playerArmor);
        // **从 `Id` 重新创建 `Card` 并恢复 `deckManager.deck`**
        List<Card> restoredDeck = new List<Card>();
        foreach (string cardId in gameData.playerDeckIds)
        {
            Card restoredCard = CardDatabase.Instance.GetCardById(cardId)?.Clone();
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
        Player.Instance.deckManager.RefreshCardReferences(Player.Instance, Player.Instance.monsterManager);
    }


    
}
