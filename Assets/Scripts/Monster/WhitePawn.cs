using UnityEngine;
using System.Collections.Generic;

public class WhitePawn : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "WhitePawn";
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

        Vector2Int direction = player.position - position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 所有可能的移动方向：上下左右
        possibleMoves.Add(new Vector2Int(position.x + 1, position.y));  // 右
        possibleMoves.Add(new Vector2Int(position.x - 1, position.y));  // 左
        possibleMoves.Add(new Vector2Int(position.x, position.y + 1));  // 上
        possibleMoves.Add(new Vector2Int(position.x, position.y - 1));  // 下

        // 按照接近玩家的优先级排序
        possibleMoves.Sort((a, b) => Vector2Int.Distance(a, player.position).CompareTo(Vector2Int.Distance(b, player.position)));

        // 遍历所有可能的移动方向，找到第一个有效的移动
        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPosition(move))
            {
                position = move;
                UpdatePosition();
                break;
            }
        }

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player attacked by WhitePawn.");
            //player.TakeDamage(1);  // 假设每次攻击造成 1 点伤害
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/WhitePawn");
    }
}
