using UnityEngine;
using System.Collections.Generic;

public static class MoveHelper
{
    public static void ShowRookMoveOptions(Player player, Card card)
    {
        if (player == null)
        {
            Debug.LogError("Player is null in ShowRookMoveOptions.");
            return;
        }

        if (card == null)
        {
            Debug.LogError("Card is null in ShowRookMoveOptions.");
            return;
        }

        player.ClearMoveHighlights();
        List<Vector2Int> validPositions = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), // 横向移动
            new Vector2Int(0, 1), new Vector2Int(0, -1)  // 纵向移动
        };

        Debug.Log($"Current Location: {player.position}");

        foreach (Vector2Int direction in directions)
        {
            Vector2Int currentPos = player.position;
            for (int i = 1; i < player.boardSize; i++)
            {
                Vector2Int newPosition = currentPos + direction * i;
                if (!player.IsValidPosition(newPosition))
                {
                    Debug.Log($"Invalid position: {newPosition}");
                    break;
                }
                if (IsBlockedByMonster(newPosition))
                {
                    Debug.Log($"Blocked by monster at position: {newPosition}");
                    break;
                }
                validPositions.Add(newPosition);
            }
        }

        List<Vector2Int> rookDirections = new List<Vector2Int>();
        foreach (var pos in validPositions)
        {
            // 计算每个有效位置相对于玩家位置的偏移量
            Vector2Int relativeDirection = new Vector2Int(pos.x - player.position.x, pos.y - player.position.y);
            rookDirections.Add(relativeDirection);
        }

        player.ShowMoveOptions(rookDirections.ToArray(), card);
    }

    public static void ShowBishopMoveOptions(Player player, Card card)
    {
        if (player == null)
        {
            Debug.LogError("Player is null in ShowBishopMoveOptions.");
            return;
        }

        if (card == null)
        {
            Debug.LogError("Card is null in ShowBishopMoveOptions.");
            return;
        }

        player.ClearMoveHighlights();
        List<Vector2Int> validPositions = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        Debug.Log($"Current Location: {player.position}");

        foreach (Vector2Int direction in directions)
        {
            Vector2Int currentPos = player.position;
            for (int i = 1; i < player.boardSize; i++)
            {
                Vector2Int newPosition = currentPos + direction * i;
                if (!player.IsValidPosition(newPosition))
                {
                    Debug.Log($"Invalid position: {newPosition}");
                    break;
                }
                if (IsBlockedByMonster(newPosition))
                {
                    Debug.Log($"Blocked by monster at position: {newPosition}");
                    break;
                }
                validPositions.Add(newPosition);
            }
        }

        List<Vector2Int> bishopDirections = new List<Vector2Int>();
        foreach (var pos in validPositions)
        {
            // 计算每个有效位置相对于玩家位置的偏移量
            Vector2Int relativeDirection = new Vector2Int(pos.x - player.position.x, pos.y - player.position.y);
            bishopDirections.Add(relativeDirection);
        }

        player.ShowMoveOptions(bishopDirections.ToArray(), card);
    }

    private static bool IsBlockedByMonster(Vector2Int position)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Slime slime = monster.GetComponent<Slime>();
            if (slime != null && slime.position == position)
            {
                return true;
            }
        }
        return false;
    }
}