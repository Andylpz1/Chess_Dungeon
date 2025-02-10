using System.Collections.Generic;

[System.Serializable]
public class LevelConfig
{
    public int levelNumber;
    public List<string> monsterTypes; // 怪物类型名称的列表，例如 "Slime", "Bat"
    public string terrainType = "Plain";
}

[System.Serializable]
public class TerrainConfig
{
    public string name;
    public int mapSize = 8;
    public int openAreaSize;  // 对于“边缘地”有效
    public int randomObstacleCount;  // 对于“采石场”有效
    public string obstacleType;
}


[System.Serializable]
public class GameConfig
{
    public List<LevelConfig> levels;
    public List<TerrainConfig> terrains;
}
