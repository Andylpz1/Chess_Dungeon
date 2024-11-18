using UnityEngine;
using System.Collections.Generic;

public class Hound : Monster
{
    public Hound()
    {
        health = 1; // 设置血量
    }

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "Hound";
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

        // 添加所有可能的移动方向
        possibleMoves.Add(new Vector2Int(position.x + 2 * (int)Mathf.Sign(direction.x), position.y));
        possibleMoves.Add(new Vector2Int(position.x, position.y + 2 * (int)Mathf.Sign(direction.y)));
        possibleMoves.Add(new Vector2Int(position.x - 2 * (int)Mathf.Sign(direction.x), position.y));
        possibleMoves.Add(new Vector2Int(position.x, position.y - 2 * (int)Mathf.Sign(direction.y)));

        Vector2Int bestMove = position; // 初始值为当前位置
        float shortestDistance = Vector2Int.Distance(position, player.position);

        // 遍历所有可能的移动方向，选择距离玩家最近的那个位置
        foreach (Vector2Int move in possibleMoves)
        {
            if (IsValidPosition(move) && !IsPositionOccupied(move))
            {
                float distance = Vector2Int.Distance(move, player.position);
                if (distance < shortestDistance)
                {
                    bestMove = move;
                    shortestDistance = distance;
                }
            }
        }

        position = bestMove;
        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player touched by Hound. Game Over.");
            // 在此处添加游戏结束逻辑
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/Hound");
    }
}
