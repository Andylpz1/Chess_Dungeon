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
        if (player == null)
        {
            Debug.LogError("Player reference is null in MoveHighlight. Ensure Initialize is called before interaction.");
            return;
        }

        if (isMove)
        {
            Debug.Log($"Moving player to position {position}");
            player.Move(position);
        }
        else
        {
            if (player.currentCard is FlailCard flailCard)
            {
                Debug.Log($"Performing flail attack at position {position}");
                List<Vector2Int> attackPositions = flailCard.GetAttackPositions(player.position, position, player.boardSize);
                player.MultipleAttack(attackPositions.ToArray());
            }
            else
            {
                Debug.Log($"Attacking position {position}");
                player.Attack(position);
            }
        }
    }
}
