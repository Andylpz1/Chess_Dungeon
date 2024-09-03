using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MonsterManager : MonoBehaviour
{
    public int boardSize = 8;
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap

    private List<Monster> monsters = new List<Monster>();
    private List<Scene> scenes = new List<Scene>();
    private List<GameObject> warnings = new List<GameObject>();
    private List<GameObject> pointObjects = new List<GameObject>();

    private int currentLevel = 1;
    private int totalMonstersToSpawn;
    private int totalMonstersKilled;

    public Player player; // 玩家对象
    public RewardManager rewardManager;
    private List<LevelConfig> levelConfigs; // 关卡配置列表
    private Dictionary<string, GameObject> monsterPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        // Initialize the player in Awake to ensure it is set before Start
        player = FindObjectOfType<Player>();
        rewardManager = FindObjectOfType<RewardManager>();
        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }
        
        // Load all monster prefabs
        monsterPrefabs["Slime"] = Resources.Load<GameObject>("Prefabs/Monster/Slime");
        monsterPrefabs["Bat"] = Resources.Load<GameObject>("Prefabs/Monster/Bat");
        monsterPrefabs["Hound"] = Resources.Load<GameObject>("Prefabs/Monster/Hound");
        monsterPrefabs["SlimeKing"] = Resources.Load<GameObject>("Prefabs/Monster/Slime_King");

        rewardManager.OnRewardSelectionComplete += OnRewardSelectionComplete;


        LoadLevelConfigs();
    }

    void Start()
    {
        SpawnWarning();
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

    void StartLevel(int level)
    {
        // 清空之前存储的位置数据
        player.activatePointPositions.Clear();
        player.deactivatePointPositions.Clear();
        

        LevelConfig levelConfig = levelConfigs.Find(l => l.levelNumber == level);
        if (levelConfig == null)
        {
            Debug.LogError("Level configuration not found for level: " + level);
            return;
        }

        // 清除所有场景对象
        ClearAllScenes();
        ClearAllPoints();

        totalMonstersToSpawn = levelConfig.monsterTypes.Count;
        totalMonstersKilled = 0;
        SpawnMonstersForLevel(levelConfig);

        if (level > 1)
        {
            rewardManager.OpenRewardPanel();
        }
        
    }

    private void OnRewardSelectionComplete()
    {
    // Actions to perform after reward selection is complete
    // For example, shuffle the deck, check for energy cards, draw cards, etc.

    // Check if there are any energy cards in the deck after the reward is added
        player.deckManager.RestartHand();
        bool hasEnergyCard = player.deckManager.deck.Exists(card => card.isEnergy);

        // If there are energy cards, spawn ActivatePoints
        if (hasEnergyCard)
        {
            SpawnActivatepointsForLevel();
        }
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


    Monster CreateMonsterByType(string type)
    {
        if (monsterPrefabs.TryGetValue(type, out GameObject prefab))
        {
            GameObject monsterObject = Instantiate(prefab);
            return monsterObject.GetComponent<Monster>();
        }

        Debug.LogError($"Unknown or missing monster type: {type}");
        return null;
    }

    public void SpawnWarning()
    {
        // Vector2Int warningPosition = GetRandomPosition();
        // Vector3 worldPosition = CalculateWorldPosition(warningPosition);
        // GameObject warningObject = Instantiate(warningPrefab, worldPosition, Quaternion.identity);
        // warnings.Add(warningObject);
        // Debug.Log("Warning spawned at position: " + warningPosition);
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
                yield return new WaitForSeconds(0.5f); // 延迟0.5秒
            }
        }
        yield return new WaitForSeconds(0.5f); // 在所有怪物移动后延迟0.5秒
        //生成新的怪物
        if (monsters.Count == 0)
        {
            StartLevel(++currentLevel);
        }
    }

    public void OnMonsterKilled()
    {
        totalMonstersKilled++;
        if (totalMonstersKilled >= totalMonstersToSpawn)
        {
            StartLevel(++currentLevel);
        }
    }

    public void OnTurnEnd(int turnCount)
    {
        // 移除已被销毁的Monster对象
        monsters.RemoveAll(monster => monster == null);

        if (turnCount % 1 == 0)
        {
            MoveMonsters();
            Debug.Log("Monsters move.");
        }
    }

    Vector2Int GetRandomPosition(Monster monsterType)
    {
        Vector2Int playerPosition = player.position;
        Debug.Log("Player position: " + playerPosition);
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int> { playerPosition };
        // Add all occupied positions
        foreach (Monster monster in monsters)
        {
            occupiedPositions.UnionWith(monster.GetOccupiedPositions(monster.position));
        }

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
        } while (occupiedPositions.Overlaps(monsterParts) || randomPosition == restrictedPosition || !AreAllPositionsValid(monsterParts) || monsterParts.Contains(playerPosition));

        return randomPosition;
    }

    Vector2Int GetEmptyPosition()
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

    void ClearWarnings()
    {
        // foreach (GameObject warning in warnings)
        // {
        //     Destroy(warning);
        // }
        // warnings.Clear();
    }

    public Monster FindNearestMonster(Vector2Int playerPosition)
    {
        Monster nearestMonster = null;
        float nearestDistance = float.MaxValue;

        foreach (Monster monster in monsters)
        {
            float distance = Vector2Int.Distance(playerPosition, monster.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestMonster = monster;
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
