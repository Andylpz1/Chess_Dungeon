using UnityEngine;

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
            player.Attack(position);
        }
    }
}
