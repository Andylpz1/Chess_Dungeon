using UnityEngine;
using System.Collections.Generic;

public class WhiteKnight : Monster
{
    private static readonly Vector2Int[] knightMoves = new Vector2Int[]
    {
        new Vector2Int(2, 1), new Vector2Int(2, -1),
        new Vector2Int(-2, 1), new Vector2Int(-2, -1),
        new Vector2Int(1, 2), new Vector2Int(1, -2),
        new Vector2Int(-1, 2), new Vector2Int(-1, -2)
    };

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "WhiteKnight";
        type = MonsterType.Knight;
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
        Vector2Int oldPos = position;
        Vector2Int bestMove = position;
        float closestDistance = Vector2Int.Distance(position, player.position);

        // 遍历所有可能的马跳跃位置
        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int potentialPosition = position + move;
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
        // 计算这次移动的向量（用于记录进攻方向）
        Vector2Int knightMove = bestMove - oldPos;
        position = bestMove;
        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            lastRelativePosition = ComputeKnightPushDirection(knightMove);;
            Debug.Log("Knight attacked the player. lastRelativePosition set to knight move vector: " + lastRelativePosition);
        }
        else
        {
            lastRelativePosition = position - player.position;
        }
    }

    private Vector2Int ComputeKnightPushDirection(Vector2Int knightMove)
    {
        // 如果垂直分量更大，则以垂直方向为主
        if (Mathf.Abs(knightMove.y) > Mathf.Abs(knightMove.x))
        {
            return new Vector2Int(0, (int)Mathf.Sign(-knightMove.y));
        }
        else
        {
            // 否则以水平方向为主
            return new Vector2Int((int)Mathf.Sign(-knightMove.x), 0);
        }
    }


    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/WhiteKnight");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        foreach (Vector2Int move in knightMoves)
        {
            Vector2Int potentialPosition = position + move;
            if (IsValidPosition(potentialPosition) && !IsPositionOccupied(potentialPosition))
            {
                possibleMoves.Add(potentialPosition);
            }
        }

        return possibleMoves;
    }
}
