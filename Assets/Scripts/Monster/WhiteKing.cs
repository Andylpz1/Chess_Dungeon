using UnityEngine;
using System.Collections.Generic;

public class WhiteKing : Monster
{
    // 定义国王的所有可能移动方向（仅限于周围一格）
    private static readonly Vector2Int[] kingDirections = new Vector2Int[]
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),  // 水平方向
        new Vector2Int(0, 1), new Vector2Int(0, -1),  // 垂直方向
        new Vector2Int(1, 1), new Vector2Int(1, -1),  // 对角线方向
        new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    private MonsterManager monsterManager;  // 用于召唤新的 Pawn

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "WhiteKing";
        monsterManager = FindObjectOfType<MonsterManager>();  
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
    }

    public override void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2Int bestMove = position;
        float closestDistance = Vector2Int.Distance(position, player.position);

        // 遍历所有可能的国王移动方向（每次只能移动一格）
        foreach (Vector2Int direction in kingDirections)
        {
            Vector2Int potentialPosition = position + direction;

            if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
            {
                float distanceToPlayer = Vector2Int.Distance(potentialPosition, player.position);
                if (distanceToPlayer < closestDistance)
                {
                    bestMove = potentialPosition;
                    closestDistance = distanceToPlayer;
                }
            }
        }

        position = bestMove;
        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player attacked by WhiteKing.");
            //player.TakeDamage(1); // 假设每次攻击造成1点伤害
        }
        SummonPawn();
    }

    private void SummonPawn()
    {
        Debug.Log("nooooo");
        if (monsterManager != null)
        {
            Debug.Log("yes");
            Monster pawn = monsterManager.CreateMonsterByType("WhitePawn");
            if (pawn != null)
            {
                Vector2Int spawnPosition = FindValidSpawnPosition();
                if (IsValidPosition(spawnPosition) && !IsPositionOccupied(spawnPosition))
                {
                    pawn.Initialize(spawnPosition);
                    monsterManager.SpawnMonster(pawn);
                    Debug.Log("WhiteKing summoned a WhitePawn at " + spawnPosition);
                }
            }
        }
    }

    private Vector2Int FindValidSpawnPosition()
    {
        // 简单地选择国王周围的一个空格
        foreach (Vector2Int direction in kingDirections)
        {
            Vector2Int potentialPosition = position + direction;
            if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
            {
                return potentialPosition;
            }
        }
        return position;  // 如果找不到空格，返回当前位置（不会实际用到）
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/WhiteKing");
    }
}
