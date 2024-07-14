using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public MonsterManager monsterManager;
    public int turnCount = 0;

    void Start()
    {
        if (monsterManager == null)
        {
            monsterManager = FindObjectOfType<MonsterManager>();
        }

        // 在游戏开始时手动生成一个史莱姆
        if (monsterManager != null)
        {
            monsterManager.SpawnSlime();
        }
    }

    public void AdvanceTurn()
    {
        turnCount++;
        //结束回合
        if (monsterManager != null)
        {
            monsterManager.OnTurnEnd(turnCount);
        }

        if (turnCount % 3 == 0 && monsterManager != null)
        {
            monsterManager.SpawnSlime();
        }
    }
}
