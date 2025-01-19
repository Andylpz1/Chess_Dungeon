using System.IO;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int playerHealth;
    public List<string> playerDeck; // 卡组中每张卡牌的名称
    public int currentLevel; // 当前关卡编号
}
