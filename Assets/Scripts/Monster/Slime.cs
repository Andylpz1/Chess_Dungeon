using UnityEngine;
using System.Collections.Generic;


public class Slime : Monster
{
    
    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "Slime";
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

        // 优先沿着 x 方向移动
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            possibleMoves.Add(new Vector2Int(position.x + (int)Mathf.Sign(direction.x), position.y));
            possibleMoves.Add(new Vector2Int(position.x, position.y + (int)Mathf.Sign(direction.y)));
        }
        else // 优先沿着 y 方向移动
        {
            possibleMoves.Add(new Vector2Int(position.x, position.y + (int)Mathf.Sign(direction.y)));
            possibleMoves.Add(new Vector2Int(position.x + (int)Mathf.Sign(direction.x), position.y));
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
            Debug.Log("Player touched by Slime. Game Over.");
            // 在此处添加游戏结束逻辑
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/Slime");
    }
}
