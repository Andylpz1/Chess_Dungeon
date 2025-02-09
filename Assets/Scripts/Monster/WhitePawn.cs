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

        Vector2Int originalPosition = position;
        Vector2Int direction = player.position - position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 根据玩家的位置计算可能的移动方向（上下左右）
        if (direction.x != 0)
        {
            possibleMoves.Add(new Vector2Int(position.x + (int)Mathf.Sign(direction.x), position.y)); // 左或右移动
        }
        if (direction.y != 0)
        {
            possibleMoves.Add(new Vector2Int(position.x, position.y + (int)Mathf.Sign(direction.y))); // 上或下移动
        }

        // 尝试每一个可能的移动方向，直到找到一个未被占据的位置
        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPosition(move))
            {
                position = move;
                break;
            }
        }

        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player attacked by WhitePawn.");
            // 在此处添加攻击玩家的逻辑，比如减少玩家的生命值等
            //player.TakeDamage(1); // 假设每次攻击造成1点伤害
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/WhitePawn");
    }
}
