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
            if (player.currentCard is FlailCard flailCard)
            {
                List<Vector2Int> attackPositions = flailCard.GetAttackPositions(player.position, position, player.boardSize);
                player.MultipleAttack(attackPositions.ToArray());
            }
            else
            {
                player.Attack(position);
            }
        }
    }
}
