using UnityEngine;
using System.Collections.Generic;

public class WhiteQueen : Monster
{
    // 定义皇后的所有可能移动方向（直线 + 对角线）
    private static readonly Vector2Int[] queenDirections = new Vector2Int[]
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),  // 水平方向
        new Vector2Int(0, 1), new Vector2Int(0, -1),  // 垂直方向
        new Vector2Int(1, 1), new Vector2Int(1, -1),  // 对角线方向
        new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "WhiteQueen";
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
        Vector2Int chosenDirection = Vector2Int.zero;
        // 遍历所有可能的皇后移动方向
        foreach (Vector2Int direction in queenDirections)
        {
            Vector2Int potentialPosition = position;

            // 沿着当前方向移动直到遇到障碍或越界
            while (true)
            {
                potentialPosition += direction;

                if (!IsValidPosition(potentialPosition) || IsPositionOccupied(potentialPosition))
                    break;

                float distanceToPlayer = Vector2Int.Distance(potentialPosition, player.position);
                if (distanceToPlayer < closestDistance)
                {
                    bestMove = potentialPosition;
                    closestDistance = distanceToPlayer;
                    chosenDirection = direction; 
                }
            }
        }

        position = bestMove;
        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            lastRelativePosition = -chosenDirection;
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/WhiteQueen");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
            // 水平和垂直方向（车的方向）
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
        
            // 对角线方向（象的方向）
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int currentPos = position + direction;
            while (IsValidPosition(currentPos) && !IsPositionOccupied(currentPos))
            {
                possibleMoves.Add(currentPos);
                currentPos += direction;
            }
        }

        return possibleMoves;
    }
}
