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

        position = bestMove;
        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player attacked by WhiteKnight.");
            //player.TakeDamage(1); // 假设每次攻击造成1点伤害
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
