using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LocationManager : MonoBehaviour
{
    private List<Vector2Int> nonEnterablePositions = new List<Vector2Int>();
    private List<Vector2Int> EnterablePositions = new List<Vector2Int>();
    private Dictionary<string, GameObject> locationPrefabs = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedLocations = new List<GameObject>();  // 动态生成的障碍物

    public List<FirePoint> activeFirePoints = new List<FirePoint>();
    public List<FireZone> activeFireZones = new List<FireZone>(); // 火域列表
    private Player player;
    private List<TerrainConfig> terrainConfigs = new List<TerrainConfig>();


    void Awake()
    {
        player = FindObjectOfType<Player>(); 
        activeFireZones = new List<FireZone>();
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
        else if (terrainType == "ForestMaze")
        {
            GenerateForestMaze();
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

    /// <summary>
    /// “林中密道”示例：墙壁位置放 Forest，所有 F 和 . 都留空，跳过 (4,4)。
    /// ASCII map:
    /// ########
    /// #F..F..#
    /// #.##.#.#
    /// #...F..#
    /// #.##.#.#
    /// #..F..F#
    /// #F....F#
    /// ########
    /// </summary>
    private void GenerateForestMaze()
    {

        // ASCII 关卡布局
        string[] rows = new string[]
        {
            "########",
            "#F.....#",
            "#.##.#.#",
            "#...F..#",
            "#.##.#.#",
            "#.#F.#F#",
            "#F...#F#",
            "########"
        };

        GameObject forestPrefab = locationPrefabs["Forest"];
        int size = rows.Length;  // 应当为 8

        for (int y = 0; y < size; y++)
        {
            // 把 y 翻转，0->7, 1->6, …, 7->0
            int rowIndex = size - 1 - y;
            string row = rows[rowIndex];

            for (int x = 0; x < row.Length; x++)
            {
                char c = row[x];
                if (c == '#' && !(x == 4 && y == 4))
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    CreateLocation(forestPrefab, pos);
                }
            }
        }


        Debug.Log("Generated ForestMaze layout with ASCII map.");
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

    public void CreateLocation(GameObject prefab, Vector2Int position, float rotation = 0f)
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

    public void CreateFirePoint(GameObject prefab, Vector2Int position, float rotation = 0f)
    {
        GameObject firePointObject = Instantiate(prefab);
        firePointObject.transform.position = player.CalculateWorldPosition(position);
        firePointObject.transform.rotation = Quaternion.Euler(0, 0, rotation); // 设置旋转

        FirePoint firePoint = firePointObject.GetComponent<FirePoint>();
        if (firePoint != null)
        {
            firePoint.Initialize(position, "fire point", true);
            spawnedLocations.Add(firePointObject); // 动态生成的对象加入列表
            OnFirePointAdded(firePoint);
            Debug.Log($"FirePoint created at {position} using {prefab.name} with rotation {rotation}");
        }
        else
        {
        Debug.LogWarning($"Prefab {prefab.name} does not contain a FirePoint component.");
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

    public void OnFirePointAdded(FirePoint newFirePoint)
    {
        if (!activeFirePoints.Contains(newFirePoint))
        {
            activeFirePoints.Add(newFirePoint);
            CheckAndFormFireZone();
        }
    }
    
    public void RemoveFirePoint(FirePoint firePoint)
    {
        if (activeFirePoints.Contains(firePoint))
        {
            activeFirePoints.Remove(firePoint);
        }
    }
    
    /// <summary>
    /// 清除所有当前存在的火域视觉
    /// </summary>
    private void ClearFireZones()
    {
        foreach (FireZone zone in activeFireZones)
        {
            if (zone != null)
                Destroy(zone.gameObject);
        }
        activeFireZones.Clear();
    }

    /// <summary>
    /// 检查所有燃点，寻找连通区域，并生成火域；火域不会消除燃点（按照新要求）。
    /// 当有新的燃点时，先清除已有火域，再重新连接所有燃点。
    /// </summary>
    public void CheckAndFormFireZone()
    {
        // 先清除现有火域
        ClearFireZones();
        
        List<FirePoint> processed = new List<FirePoint>();
        foreach (FirePoint fp in new List<FirePoint>(activeFirePoints))
        {
            if (!processed.Contains(fp))
            {
                List<FirePoint> cluster = GetConnectedFirePoints(fp);
                processed.AddRange(cluster);
                if (cluster.Count >= 2)
                {
                    // 根据 cluster 计算连接图形
                    List<Vector3> polygonPoints = new List<Vector3>();
                    if (cluster.Count == 2)
                    {
                        // 只有两个点，直接连线
                        Vector3 p1 = player.CalculateWorldPosition(cluster[0].gridPosition);
                        Vector3 p2 = player.CalculateWorldPosition(cluster[1].gridPosition);
                        polygonPoints.Add(p1);
                        polygonPoints.Add(p2);
                        // 为闭合方便，重复第一个点
                        polygonPoints.Add(p1);
                    }
                    else
                    {
                        // 三个或以上，计算凸包
                        polygonPoints = ComputeConvexHull(cluster);
                        // 确保闭合
                        if (polygonPoints.Count > 0 && polygonPoints[0] != polygonPoints[polygonPoints.Count - 1])
                            polygonPoints.Add(polygonPoints[0]);
                        
                    }
                    
                    if (polygonPoints.Count >= 2)
                    {
                        CreateFireZoneUsingPoints(polygonPoints);
                    }
                }
            }
        }
    }
    
    private List<FirePoint> GetConnectedFirePoints(FirePoint start)
    {
        List<FirePoint> cluster = new List<FirePoint>();
        Queue<FirePoint> queue = new Queue<FirePoint>();
        queue.Enqueue(start);
        cluster.Add(start);

        while (queue.Count > 0)
        {
            FirePoint current = queue.Dequeue();
            foreach (FirePoint fp in activeFirePoints)
            {
                //if (!cluster.Contains(fp) && IsAdjacent(current.gridPosition, fp.gridPosition))
                if (!cluster.Contains(fp))
                {
                    cluster.Add(fp);
                    queue.Enqueue(fp);
                }
            }
        }
        return cluster;
    }
    
    private bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) <= 1 && Mathf.Abs(a.y - b.y) <= 1;
    }

    /// <summary>
    /// 使用 Graham scan 算法计算凸包（返回世界坐标点）
    /// </summary>
    private List<Vector3> ComputeConvexHull(List<FirePoint> cluster)
    {
        List<Vector2> points = new List<Vector2>();
        foreach (FirePoint fp in cluster)
        {
            // 将 gridPosition 转为 Vector2，假设 CalculateWorldPosition 直接对应每格 1 单位
            points.Add(new Vector2(fp.gridPosition.x, fp.gridPosition.y));
        }
        
        if (points.Count <= 1)
            return new List<Vector3>();

        // 找到最下方（如果相同，则最左）的点作为 pivot
        Vector2 pivot = points[0];
        foreach (Vector2 p in points)
        {
            if (p.y < pivot.y || (p.y == pivot.y && p.x < pivot.x))
                pivot = p;
        }
        // 按角度排序
        float Dist2(Vector2 v) => (v.x - pivot.x) * (v.x - pivot.x) + (v.y - pivot.y) * (v.y - pivot.y);
        points.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.y - pivot.y, a.x - pivot.x);
            if(angleA>180)
                angleA -= 180;
            float angleB = Mathf.Atan2(b.y - pivot.y, b.x - pivot.x);
            if (angleB > 180)
                angleB -= 180;
            if (!Mathf.Approximately(angleA, angleB))
                return angleA.CompareTo(angleB);          // 角度不同，按角度排
            else
                return Dist2(a).CompareTo(Dist2(b));      // 角度相同，按距离排，近的在前，远的在后
        });
        
        List<Vector2> hull = new List<Vector2>();
        foreach (Vector2 p in points)
        {
            while (hull.Count >= 2 && Cross(hull[hull.Count - 2], hull[hull.Count - 1], p) <0)
            {
                hull.RemoveAt(hull.Count - 1);
            }
            hull.Add(p);
        }
        
        // 转换为世界坐标（这里假设不需要额外偏移）
        List<Vector3> worldHull = new List<Vector3>();
        foreach (Vector2 p in hull)
        {
            // 使用 player.CalculateWorldPosition 对 grid 坐标转换为世界坐标
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
            worldHull.Add(player.CalculateWorldPosition(gridPos));
        }
        return worldHull;
    }
    
    private float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }

    /// <summary>
    /// 根据计算得到的多边形顶点创建火域视觉
    /// </summary>
    private void CreateFireZoneUsingPoints(List<Vector3> points)
    {
        GameObject zoneObj = new GameObject("FireZone");
        zoneObj.transform.SetParent(transform);
        zoneObj.transform.position = Vector3.zero; // 使用世界坐标

        // 添加 LineRenderer 组件
        LineRenderer lr = zoneObj.AddComponent<LineRenderer>();
        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        lr.widthMultiplier = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.useWorldSpace = true;

        // 添加 FireZone 组件，用于管理持续时间与后续效果
        FireZone fireZone = zoneObj.AddComponent<FireZone>();
        // 使用重载的 Initialize 方法接收多边形顶点，设置持续时间，比如 2 个敌方回合
        fireZone.Initialize(points, 2);
        activeFireZones.Add(fireZone);

        Debug.Log($"Created FireZone with {points.Count} vertices.");
    }
}