using UnityEngine;
using System.Collections.Generic;

public class FlailCard : Card
{
    public FlailCard() : base(CardType.Attack, "A05", 60) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/flail_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/flail_card");
    }

    public List<Vector2Int> GetAttackPositions(Vector2Int playerPosition, Vector2Int targetPosition, int boardSize)
    {
        List<Vector2Int> attackPositions = new List<Vector2Int>();

        // 计算方向
        Vector2Int direction = targetPosition - playerPosition;

        // 添加主攻击位置
        Vector2Int mainAttackPosition = targetPosition;
        if (IsValidPosition(mainAttackPosition, boardSize))
        {
            attackPositions.Add(mainAttackPosition);
        }

        // 添加左右攻击位置
        if (direction == Vector2Int.up || direction == Vector2Int.down)
        {
            Vector2Int leftPosition = mainAttackPosition + Vector2Int.left;
            Vector2Int rightPosition = mainAttackPosition + Vector2Int.right;

            if (IsValidPosition(leftPosition, boardSize))
            {
                attackPositions.Add(leftPosition);
            }
            if (IsValidPosition(rightPosition, boardSize))
            {
                attackPositions.Add(rightPosition);
            }
        }
        else if (direction == Vector2Int.left || direction == Vector2Int.right)
        {
            Vector2Int upPosition = mainAttackPosition + Vector2Int.up;
            Vector2Int downPosition = mainAttackPosition + Vector2Int.down;

            if (IsValidPosition(upPosition, boardSize))
            {
                attackPositions.Add(upPosition);
            }
            if (IsValidPosition(downPosition, boardSize))
            {
                attackPositions.Add(downPosition);
            }
        }

        return attackPositions;
    }

    private bool IsValidPosition(Vector2Int position, int boardSize)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }
}
