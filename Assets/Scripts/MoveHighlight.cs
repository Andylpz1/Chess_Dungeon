using UnityEngine;
using System.Collections.Generic;

public class MoveHighlight : MonoBehaviour
{
    private Player player;
    private Vector2Int position;
    private bool isMove;

    public void Initialize(Player player, Vector2Int position, bool isMove)
    {
        this.player = player;
        this.position = position;
        this.isMove = isMove;
    }

    void OnMouseDown()
    {
        if (isMove)
        {
            player.Move(position);
        }
        else
        {
            //链枷特殊攻击方式
            if (player.currentCard.Id == "A05")
            {
                List<Vector2Int> attackPositions = new List<Vector2Int>();

                // 计算方向
                Vector2Int direction = position - player.position;

                // 添加主攻击位置
                Vector2Int mainAttackPosition = position;
                if (player.IsValidPosition(mainAttackPosition))
                {
                    attackPositions.Add(mainAttackPosition);
                }

                // 添加左右攻击位置
                if (direction == Vector2Int.up || direction == Vector2Int.down)
                {
                    Vector2Int leftPosition = mainAttackPosition + Vector2Int.left;
                    Vector2Int rightPosition = mainAttackPosition + Vector2Int.right;

                    if (player.IsValidPosition(leftPosition))
                    {
                        attackPositions.Add(leftPosition);
                    }
                    if (player.IsValidPosition(rightPosition))
                    {
                        attackPositions.Add(rightPosition);
                    }
                }
                else if (direction == Vector2Int.left || direction == Vector2Int.right)
                {
                    Vector2Int upPosition = mainAttackPosition + Vector2Int.up;
                    Vector2Int downPosition = mainAttackPosition + Vector2Int.down;

                    if (player.IsValidPosition(upPosition))
                    {
                        attackPositions.Add(upPosition);
                    }
                    if (player.IsValidPosition(downPosition))
                    {
                        attackPositions.Add(downPosition);
                    }
                }

                player.MultipleAttack(attackPositions.ToArray());
            }
            else
            {
                player.Attack(position);
            }
        }
    }
}
