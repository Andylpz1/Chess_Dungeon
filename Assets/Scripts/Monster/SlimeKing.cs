using UnityEngine;
using System.Collections.Generic;

public class SlimeKing : Monster
{
    private List<Vector2Int> occupiedPositions;

    public SlimeKing()
    {
        health = 5;
    }

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        UpdateOccupiedPositions();
    }

    private void UpdateOccupiedPositions()
    {
        occupiedPositions = new List<Vector2Int>
        {
            position,
            position + new Vector2Int(1, 0),
            position + new Vector2Int(-1, 0),
            position + new Vector2Int(0, 1),
            position + new Vector2Int(1, 1),
            position + new Vector2Int(-1, 1),
            position + new Vector2Int(0, -1),
            position + new Vector2Int(1, -1),
            position + new Vector2Int(-1, -1)
        };
    }

    public override void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2Int direction = player.position - position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int((int)Mathf.Sign(direction.x), 0),
            position + new Vector2Int(0, (int)Mathf.Sign(direction.y))
        };

        foreach (Vector2Int move in possibleMoves)
        {
            if (IsValidMove(move))
            {
                position = move;
                UpdateOccupiedPositions(); // Update positions after moving
                break;
            }
        }

        UpdatePosition();

        if (IsPlayerTouched())
        {
            Debug.Log("Player touched by SlimeKing. Game Over.");
            // Add game over logic here
        }
    }

    private bool IsValidMove(Vector2Int move)
    {
        foreach (Vector2Int pos in GetOccupiedPositions(move))
        {
            if (!IsValidPosition(pos) || IsPositionOccupied(pos))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsPlayerTouched()
    {
        return occupiedPositions.Contains(player.position);
    }

    public override bool IsPartOfMonster(Vector2Int pos)
    {
        return occupiedPositions.Contains(pos);
    }

    public override List<Vector2Int> GetOccupiedPositions(Vector2Int position)
    {
        return new List<Vector2Int>
        {
            position,
            position + new Vector2Int(1, 0),
            position + new Vector2Int(-1, 0),
            position + new Vector2Int(0, 1),
            position + new Vector2Int(1, 1),
            position + new Vector2Int(-1, 1),
            position + new Vector2Int(0, -1),
            position + new Vector2Int(1, -1),
            position + new Vector2Int(-1, -1)
        };
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/slime_king");
    }
}
