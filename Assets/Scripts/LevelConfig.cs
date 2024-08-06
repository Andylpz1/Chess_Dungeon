using System.Collections.Generic;

[System.Serializable]
public class LevelConfig
{
    public int levelNumber;
    public List<string> monsterTypes; // 怪物类型名称的列表，例如 "Slime", "Bat"
}

[System.Serializable]
public class GameConfig
{
    public List<LevelConfig> levels;
}
