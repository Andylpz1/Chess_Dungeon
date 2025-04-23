using System;
using System.Collections.Generic;

[Serializable]
public class LevelConfig
{
    public int levelNumber;
    public List<string> monsterTypes; // 怪物类型名称的列表，例如 "Slime", "Bat"
    public List<MonsterTemplate> monsterTemplates = new List<MonsterTemplate>();
    public string terrainType = "Plain";
}

[Serializable]
public class MonsterTemplate
{
    // the list of monster type IDs for this particular template
    public List<string> monsterTypes = new List<string>();
}

[Serializable]
public class TerrainConfig
{
    public string name;
    public int mapSize = 8;
    public int openAreaSize;  // 对于“边缘地”有效
    public int randomObstacleCount;  // 对于“采石场”有效
    public string obstacleType;
}


[Serializable]
public class GameConfig
{
    public List<LevelConfig> levels;
    public List<TerrainConfig> terrains;
}
