using UnityEngine;
using System.Collections.Generic;

public class WhiteBishop : Monster
{
    // 定义四个对角线方向
    private static readonly Vector2Int[] bishopDirections = new Vector2Int[]
    {
        new Vector2Int(1, 1), new Vector2Int(1, -1),
        new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "WhiteBishop";
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

        // 遍历所有可能的对角线方向
        foreach (Vector2Int direction in bishopDirections)
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
                }
            }
        }

        position = bestMove;
        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player attacked by WhiteBishop.");
            //player.TakeDamage(1); // 假设每次攻击造成1点伤害
        }
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/WhiteBishop");
    }

    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 1),   // 右上
            new Vector2Int(-1, 1),  // 左上
            new Vector2Int(1, -1),  // 右下
            new Vector2Int(-1, -1)  // 左下
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
