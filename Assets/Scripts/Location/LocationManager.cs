using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LocationManager : MonoBehaviour
{
    private List<Vector2Int> nonEnterablePositions = new List<Vector2Int>();
    private Dictionary<string, GameObject> locationPrefabs = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedLocations = new List<GameObject>();  // 动态生成的障碍物

    private Player player;
    private List<TerrainConfig> terrainConfigs = new List<TerrainConfig>();


    void Awake()
    {
        player = FindObjectOfType<Player>(); 
        // 动态加载所有地点的 Prefab
        locationPrefabs["Forest"] = Resources.Load<GameObject>("Prefabs/Location/Forest");
        locationPrefabs["Wall_Horizontal"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Horizontal");
        locationPrefabs["Wall_Vertical"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Vertical");
        locationPrefabs["Wall_Corner_UL"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_UL"); // 左上角
        locationPrefabs["Wall_Corner_UR"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_UR"); // 右上角
        locationPrefabs["Wall_Corner_LL"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_LL"); // 左下角
        locationPrefabs["Wall_Corner_LR"] = Resources.Load<GameObject>("Prefabs/Location/Wall_Corner_LR"); // 右下角

    
        LoadTerrainConfigs();
        // 缓存场景中初始的不可进入位置
        CacheExistingLocations();
    }

    private void LoadTerrainConfigs()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Configs", "levelConfig.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameConfig gameConfig = JsonUtility.FromJson<GameConfig>(json);
            terrainConfigs = gameConfig.terrains;
        }
        else
        {
            Debug.LogError("Terrain configuration file not found: " + filePath);
        }
    }

    public void SpawnLocationsForLevel(string terrainType)
    {
        TerrainConfig terrainConfig = terrainConfigs.Find(t => t.name == terrainType);
        if (terrainConfig == null)
        {
            Debug.LogError($"Terrain configuration not found for terrain type: {terrainType}");
            return;
        }

        if (terrainType == "Plain")
        {

        }
        else if (terrainType == "Borderland")
        {
            GenerateEdgeTerrain(terrainConfig);
        }
        else if (terrainType == "FortifiedBorderland")
        {
            GenerateWallBorder(terrainConfig);
        }
        else if (terrainType == "DenseForest")
        {
            GenerateDenseForest(terrainConfig);
        }

    }

    private void GenerateEdgeTerrain(TerrainConfig config)
    {
        int mapSize = config.mapSize;
        int openAreaSize = config.openAreaSize;
        //GameObject obstaclePrefab = locationPrefabs[config.obstacleType];
        GameObject obstaclePrefab = locationPrefabs["Forest"];


        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                bool isEdge = (x < (mapSize - openAreaSize) / 2 || x >= mapSize - (mapSize - openAreaSize) / 2 ||
                               y < (mapSize - openAreaSize) / 2 || y >= mapSize - (mapSize - openAreaSize) / 2);

                if (isEdge)
                {
                    CreateLocation(obstaclePrefab, new Vector2Int(x, y));
                }
            }
        }
    }

    private void GenerateWallBorder(TerrainConfig config)
    {
        int mapSize = config.mapSize;

        GameObject wallCornerUL = locationPrefabs["Wall_Corner_UL"]; // 左上角
        GameObject wallCornerUR = locationPrefabs["Wall_Corner_UR"]; // 右上角
        GameObject wallCornerLL = locationPrefabs["Wall_Corner_LL"]; // 左下角
        GameObject wallCornerLR = locationPrefabs["Wall_Corner_LR"]; // 右下角
        GameObject wallHorizontal = locationPrefabs["Wall_Horizontal"];
        GameObject wallVertical = locationPrefabs["Wall_Vertical"];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                bool isBorder = (x == 0 || x == mapSize - 1 || y == 0 || y == mapSize - 1);

                if (isBorder)
                {
                    if (x == 0 && y == 0) // 左下角
                        CreateLocation(wallCornerLL, new Vector2Int(x, y));
                    else if (x == 0 && y == mapSize - 1) // 左上角
                        CreateLocation(wallCornerUL, new Vector2Int(x, y));
                    else if (x == mapSize - 1 && y == 0) // 右下角
                        CreateLocation(wallCornerLR, new Vector2Int(x, y));
                    else if (x == mapSize - 1 && y == mapSize - 1) // 右上角
                        CreateLocation(wallCornerUR, new Vector2Int(x, y));
                    else if (x == 0 || x == mapSize - 1) // 左右边界
                        CreateLocation(wallVertical, new Vector2Int(x, y));
                    else if (y == 0 || y == mapSize - 1) // 上下边界
                        CreateLocation(wallHorizontal, new Vector2Int(x, y));
                }
            }
        }
    }

    private void GenerateDenseForest(TerrainConfig config)
    {
        int mapSize = config.mapSize;
        int openSize = 2; // 需要留空的列数和行数
        int centerStart = (mapSize - openSize) / 2; // 计算中间空地的起始索引
        int centerEnd = centerStart + openSize; // 计算中间空地的结束索引

        GameObject forestPrefab = locationPrefabs["Forest"];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                // 只要 x 在中间两列 或 y 在中间两行，就留空；否则填充 `Forest`
                bool isForest = !(x >= centerStart && x < centerEnd) && !(y >= centerStart && y < centerEnd);

                if (isForest)
                {
                    CreateLocation(forestPrefab, new Vector2Int(x, y));
                }
            }
        }
    }





    // 缓存场景中手动摆放的不可进入位置
    private void CacheExistingLocations()
    {
        nonEnterablePositions.Clear();
        NonEnterableLocation[] nonEnterableLocations = FindObjectsOfType<NonEnterableLocation>();
        foreach (NonEnterableLocation location in nonEnterableLocations)
        {
            nonEnterablePositions.Add(location.position);
        }
        Debug.Log("Cached existing locations.");
    }

    // 清除上一关的动态生成障碍物，但保留初始手动摆放的障碍物
    public void ClearAllLocations()
    {
        foreach (GameObject location in spawnedLocations)
        {
            if (location != null)
            {
                NonEnterableLocation nonEnterableLocation = location.GetComponent<NonEnterableLocation>();
                if (nonEnterableLocation != null)
                {
                    // 从 nonEnterablePositions 中移除对应位置
                    nonEnterablePositions.Remove(nonEnterableLocation.position);
                }
                Destroy(location);
            }
        }
        spawnedLocations.Clear();
        Debug.Log("Cleared all dynamically generated locations.");
    }

    // 生成不同类型的地点
    public void GenerateLocation(string locationType, int count)
    {
        if (!locationPrefabs.ContainsKey(locationType))
        {
            Debug.LogError($"Unknown location type: {locationType}");
            return;
        }

        GameObject locationPrefab = locationPrefabs[locationType];

        for (int i = 0; i < count; i++)
        {
            Vector2Int position = GetRandomPosition();
            if (position != new Vector2Int(-1, -1))
            {
                CreateLocation(locationPrefab, position);
            }
        }
    }

    private void CreateLocation(GameObject prefab, Vector2Int position, float rotation = 0f)
    {
        GameObject locationObject = Instantiate(prefab);
        locationObject.transform.position = player.CalculateWorldPosition(position);
        locationObject.transform.rotation = Quaternion.Euler(0, 0, rotation); // 设置旋转

        NonEnterableLocation location = locationObject.GetComponent<NonEnterableLocation>();
        if (location != null)
        {
            location.Initialize(position, "A non-enterable location.", false);
            nonEnterablePositions.Add(position);  // 缓存到不可进入位置
            spawnedLocations.Add(locationObject);  // 动态生成的障碍物
            Debug.Log($"Location created at {position} using {prefab.name} with rotation {rotation}");
        }
    }


    private Vector2Int GetRandomPosition()
    {
        int boardSize = FindObjectOfType<MonsterManager>().boardSize;
        int attempts = 0;
        const int maxAttempts = 50;

        Vector2Int randomPosition;
        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            attempts++;

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Failed to find a valid location after 50 attempts.");
                return new Vector2Int(-1, -1);
            }
        } while (nonEnterablePositions.Contains(randomPosition));

        return randomPosition;
    }

    public List<Vector2Int> GetNonEnterablePositions()
    {
        return new List<Vector2Int>(nonEnterablePositions);
    }

    public bool IsNonEnterablePosition(Vector2Int position)
    {
        return nonEnterablePositions.Contains(position);
    }


}
