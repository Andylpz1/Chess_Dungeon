using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class MonsterManager : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public int boardSize = 8;
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap
    public bool nextlevel = false;

    private List<Monster> monsters = new List<Monster>();
    private List<Scene> scenes = new List<Scene>();
    private List<GameObject> warnings = new List<GameObject>();
    private List<GameObject> pointObjects = new List<GameObject>();

    private int currentLevel = 1;
    private int totalMonstersToSpawn;
    private int totalMonstersKilled;

    public Player player; // 玩家对象
    public RewardManager rewardManager;
    private LocationManager locationManager;
    private List<LevelConfig> levelConfigs; // 关卡配置列表
    private Dictionary<string, GameObject> monsterPrefabs = new Dictionary<string, GameObject>();
    public bool isLevelCompleted = false;

    public Text levelCountText;

    void Awake()
    {
        
        // Initialize the player in Awake to ensure it is set before Start
        player = FindObjectOfType<Player>();

        rewardManager = FindObjectOfType<RewardManager>();
        locationManager = FindObjectOfType<LocationManager>();
        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }
        
        // Load all monster prefabs
        monsterPrefabs["Slime"] = Resources.Load<GameObject>("Prefabs/Monster/Slime");
        monsterPrefabs["Bat"] = Resources.Load<GameObject>("Prefabs/Monster/Bat");
        monsterPrefabs["Hound"] = Resources.Load<GameObject>("Prefabs/Monster/Hound");
        monsterPrefabs["SlimeKing"] = Resources.Load<GameObject>("Prefabs/Monster/Slime_King");
        monsterPrefabs["WhitePawn"] = Resources.Load<GameObject>("Prefabs/Monster/white_pawn");
        monsterPrefabs["WhiteKnight"] = Resources.Load<GameObject>("Prefabs/Monster/white_knight");
        monsterPrefabs["WhiteBishop"] = Resources.Load<GameObject>("Prefabs/Monster/white_bishop");
        monsterPrefabs["WhiteRook"] = Resources.Load<GameObject>("Prefabs/Monster/white_rook");
        monsterPrefabs["WhiteQueen"] = Resources.Load<GameObject>("Prefabs/Monster/white_queen");
        monsterPrefabs["WhiteKing"] = Resources.Load<GameObject>("Prefabs/Monster/white_king");

        rewardManager.OnRewardSelectionComplete += OnRewardSelectionComplete;


        LoadLevelConfigs();
        
    }

    void Start()
    {
        // 读取 `LevelNode` 存储的关卡索引
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        bool isLevelNode = PlayerPrefs.GetInt("IsLevelNode", 0) == 1;
        // 如果是从 LevelNode 进入，则使用 `selectedLevel`
        if (isLevelNode)
        {
            currentLevel = selectedLevel;
            Debug.Log($"🟢 从 LevelNode 进入游戏，加载关卡 {currentLevel}");
        }
        // 否则，尝试加载存档
        else if (SaveSystem.GameSaveExists())
        {
            GameData gameData = SaveSystem.LoadGame();
            currentLevel = gameData.currentLevel;
            Debug.Log($"📀 从存档继续游戏，加载关卡 {currentLevel}");
        }
        // 如果没有存档，就使用默认值
        else
        {
            currentLevel = selectedLevel;
            Debug.Log($"🔵 没有存档，默认加载关卡 {currentLevel}");
        }

        // 开始游戏
        StartLevel(currentLevel);

    }

    void LoadLevelConfigs()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Configs", "levelConfig.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameConfig gameConfig = JsonUtility.FromJson<GameConfig>(json);
            levelConfigs = gameConfig.levels;
        }
        else
        {
            Debug.LogError("Level configuration file not found: " + filePath);
        }
    }

    private void UpdateLevelCountText()
    {
        levelCountText.text = "Level: " + currentLevel.ToString();
    }

    public void StartLevel(int level)
    {
        currentLevel = level;
        // 清除上一关的动态障碍物
        LocationManager locationManager = FindObjectOfType<LocationManager>();
        locationManager.ClearAllLocations();

        // 获取当前关卡配置并生成对应的地形
        LevelConfig levelConfig = levelConfigs.Find(l => l.levelNumber == level);
        if (levelConfig != null)
        {
            locationManager.SpawnLocationsForLevel(levelConfig.terrainType);
        }

        // 清空之前存储的位置数据
        player.activatePointPositions.Clear();
        player.deactivatePointPositions.Clear();

        player.isCharged = false;
        player.UpdateEnergyStatus();
        
        if (levelConfig == null)
        {
            Debug.LogError("Level configuration not found for level: " + level);
            return;
        }

        // 更新 UI 上的 LevelCount 文本
        UpdateLevelCountText();

        // 清除所有场景对象
        ClearAllMonsters();
        ClearAllScenes();
        ClearAllPoints();

        //生成场景
        //locationManager.GenerateLocation("Forest", 5);

        //生成怪物
        totalMonstersToSpawn = levelConfig.monsterTypes.Count;
        totalMonstersKilled = 0;
        SpawnMonstersForLevel(levelConfig);

        

        //if (level > 1)
        //{
          //  rewardManager.OpenRewardPanel();
        //}
        
    }

    public int GetCurrentLevel() {
        return currentLevel;
    }

    private void OnRewardSelectionComplete()
    {
    // Actions to perform after reward selection is complete
    // For example, shuffle the deck, check for energy cards, draw cards, etc.

    // Check if there are any energy cards in the deck after the reward is added
        rewardManager.isRewardPanelOpen = false;
        player.deckManager.RestartHand();
        bool hasEnergyCard = player.deckManager.deck.Exists(card => card.isEnergy);

        // If there are energy cards, spawn ActivatePoints
        if (isLevelCompleted)
        {
            // 如果关卡完成，则重置手牌
            player.deckManager.RestartHand();
            // 如果有能量卡，则生成激活点
            if (hasEnergyCard)
            {
                SpawnActivatepointsForLevel();
            }

            // 抓新手牌
            //player.deckManager.DrawCards(player.deckManager.handSize);

            // 重置关卡完成标记
            isLevelCompleted = false;
        }
    }
    public void ClearAllMonsters()
    {
        foreach (Monster monster in monsters)
        {
            if (monster != null)
            {
                Destroy(monster.gameObject); // Destroy the monster GameObject
            }
        }
        monsters.Clear(); // Clear the list of monsters
        Debug.Log("All monsters cleared.");
    }

    void ClearAllScenes()
    {
        foreach (Scene scene in scenes)
        {
            if (scene != null)
            {
                Destroy(scene.gameObject);
            }
        }
        scenes.Clear(); // 清空列表
    }

    void ClearAllPoints()
    {
        foreach (GameObject point in pointObjects)
        {
            Destroy(point);
        }
        pointObjects.Clear(); // 清空列表
    }



    void SpawnMonstersForLevel(LevelConfig levelConfig)
    {
        foreach (string monsterType in levelConfig.monsterTypes)
        {
            Monster monster = CreateMonsterByType(monsterType);
            if (monster != null)
            {
                SpawnMonster(monster);
            }
        }
    }

    void SpawnActivatepointsForLevel()
    {
        // 生成 6 个 ActivatePoint
        for (int i = 0; i < 6; i++)
        {
            Vector2Int position = GetEmptyPosition();
            Vector3 worldPosition = player.CalculateWorldPosition(position);
            if (position != new Vector2Int(-1, -1))
            {
                // 实例化 ActivatePoint 的预制体
                GameObject activatePointPrefab = Resources.Load<GameObject>("Prefabs/UI/ActivatePoint");
                if (activatePointPrefab != null)
                {
                    GameObject activatePointInstance = Instantiate(activatePointPrefab, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
                    ActivatePoint activatePoint = activatePointInstance.GetComponent<ActivatePoint>();
                    if (activatePoint != null)
                    {
                        //activatePoint.Initialize(position);
                        
                    }
                    player.activatePointPositions.Add(position);
                    pointObjects.Add(activatePointInstance); // 添加到列表
                }
                else
                {
                    Debug.LogError("ActivatePoint prefab not found.");
                }
            }   
        }

        // 生成 12 个 DeactivatePoint
        for (int i = 0; i < 12; i++)
        {
            Vector2Int position = GetEmptyPosition();
            Vector3 worldPosition = player.CalculateWorldPosition(position);
            if (position != new Vector2Int(-1, -1))
            {
                // 实例化 DeactivatePoint 的预制体
                GameObject deactivatePointPrefab = Resources.Load<GameObject>("Prefabs/UI/DeactivatePoint");
                if (deactivatePointPrefab != null)
                {
                    GameObject deactivatePointInstance = Instantiate(deactivatePointPrefab, new Vector3(worldPosition.x, worldPosition.y, 0), Quaternion.identity);
                    DeactivatePoint deactivatePoint = deactivatePointInstance.GetComponent<DeactivatePoint>();
                    if (deactivatePoint != null)
                    {
                        //deactivatePoint.Initialize(position);
                        
                    }
                    player.deactivatePointPositions.Add(position); // 添加位置信息到列表中
                    pointObjects.Add(deactivatePointInstance); // 添加到列表
                }
                else
                {
                    Debug.LogError("DeactivatePoint prefab not found.");
                }
            }
        }
    }


    public Monster CreateMonsterByType(string type)
    {
        if (monsterPrefabs.TryGetValue(type, out GameObject prefab))
        {
            GameObject monsterObject = Instantiate(prefab);
            return monsterObject.GetComponent<Monster>();
        }

        Debug.LogError($"Unknown or missing monster type: {type}");
        return null;
    }


    public void SpawnMonster(Monster monster)
    {
        Vector2Int spawnPosition = GetRandomPosition(monster);
        if (spawnPosition == new Vector2Int(-1, -1))
        {
            Debug.LogWarning("Failed to spawn monster: No valid position found.");
            Destroy(monster.gameObject);
            return;
        }

        Vector3 worldPosition = player.CalculateWorldPosition(spawnPosition);
        monster.transform.position = worldPosition;
        monster.Initialize(spawnPosition);
        monsters.Add(monster);
        Debug.Log($"{monster.GetType().Name} spawned at position: " + spawnPosition);
    }

    public void MoveMonsters()
    {
        StartCoroutine(MoveMonstersSequentially());
    }

    private IEnumerator MoveMonstersSequentially()
    {
        List<Monster> monstersCopy = new List<Monster>(monsters);
        foreach (Monster monster in monstersCopy)
        {
            if (monster != null)
            {
                monster.MoveTowardsPlayer();
                yield return new WaitForSeconds(0.3f); // 延迟0.5秒
            }
        }
        //yield return new WaitForSeconds(0.5f); // 在所有怪物移动后延迟0.5秒
        //生成新的怪物
        
    }

    public void OnMonsterKilled()
    {
        totalMonstersKilled++;
        if (totalMonstersKilled >= totalMonstersToSpawn)
        {
            StartLevel(++currentLevel);
        }
    }
    //回合结束检查是否进入下一个回合/开始新关卡
    public void OnTurnEnd(int turnCount)
    {
        // 移除已被销毁的Monster对象
        monsters.RemoveAll(monster => monster == null);
        nextlevel = true;
        if (monsters.Count == 0)
        {
            isLevelCompleted = true; // 标记关卡完成
            rewardManager.OpenRewardPanel();
            //StartLevel(++currentLevel);
        }
        else if (turnCount % 1 == 0 && monsters.Count != 0)
        {
            MoveMonsters();
            Debug.Log("Monsters move.");
        }
    }

    public Vector2Int GetRandomPosition(Monster monsterType)
    {
        Vector2Int playerPosition = player.position;
        Debug.Log("Player position: " + playerPosition);
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int> { playerPosition };
        // Add all occupied positions
        foreach (Monster monster in monsters)
        {
            occupiedPositions.UnionWith(monster.GetOccupiedPositions(monster.position));
        }

        // 从 LocationManager 获取不可进入位置
        HashSet<Vector2Int> nonEnterablePositions = new HashSet<Vector2Int>(FindObjectOfType<LocationManager>().GetNonEnterablePositions());

        Vector2Int restrictedPosition = new Vector2Int(3, 3); // 永远不会生成的位置

        Vector2Int randomPosition;
        List<Vector2Int> monsterParts;
        int attempts = 0;
        const int maxAttempts = 50;

        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            monsterParts = monsterType.GetOccupiedPositions(randomPosition);
            attempts++;
        
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Failed to find a valid spawn position after 50 attempts.");
                return new Vector2Int(-1, -1); // Return an invalid position to indicate failure
            }
        } while (occupiedPositions.Overlaps(monsterParts) || 
         randomPosition == restrictedPosition || 
         !AreAllPositionsValid(monsterParts) || 
         monsterParts.Contains(playerPosition) || 
         monsterParts.Exists(part => locationManager.IsNonEnterablePosition(part)));  // 确认位置是否是不可进入的

        return randomPosition;
    }

    public Vector2Int GetEmptyPosition()
    {
        Vector2Int playerPosition = player.position;
        Debug.Log("Player position: " + playerPosition);
    
        // 获取所有被占据的位置（包括怪物和场景物体）
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int> { playerPosition };
    
        // 添加所有怪物占据的位置
        foreach (Monster monster in monsters)
        {
            occupiedPositions.UnionWith(monster.GetOccupiedPositions(monster.position));
        }

        // 添加场景物体占据的位置
        foreach (Scene scene in scenes)
        {
            occupiedPositions.UnionWith(scene.GetOccupiedPositions(scene.position));
        }

        //添加所有场景占据的位置
        if (locationManager != null)
        {
            occupiedPositions.UnionWith(locationManager.GetNonEnterablePositions());
        }

        Vector2Int randomPosition;
        int attempts = 0;
        const int maxAttempts = 50;

        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            attempts++;
        
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Failed to find a valid empty position after 50 attempts.");
                return new Vector2Int(-1, -1); // Return an invalid position to indicate failure
            }
        } while (occupiedPositions.Contains(randomPosition));

        return randomPosition;
    }


    bool AreAllPositionsValid(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            if (pos.x < 0 || pos.x >= boardSize || pos.y < 0 || pos.y >= boardSize)
            {
                return false;
            }
        }
        return true;
    }

    public int GetMonsterCount()
    {
        return monsters.Count;
    }

    public Monster FindNearestMonster(Vector2Int playerPosition, bool isAdjacent = false)
    {
        Monster nearestMonster = null;
        float nearestDistance = float.MaxValue;

        foreach (Monster monster in monsters)
        {
            float distance = Vector2Int.Distance(playerPosition, monster.position);

            if (isAdjacent)
            {
                // If isAdjacent is true, we only care about monsters that are adjacent to the player
                if (Mathf.Abs(playerPosition.x - monster.position.x) <= 1 && Mathf.Abs(playerPosition.y - monster.position.y) <= 1)
                {
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestMonster = monster;
                    }
                }
            }
            else
            {
                // If isAdjacent is false, find the nearest monster regardless of adjacency
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestMonster = monster;
                }
            }
        }

        return nearestMonster;
    }


    public void RemoveMonster(Monster monster)
    {
        if (monsters.Contains(monster))
        {
            monsters.Remove(monster);
        }
    }
}
