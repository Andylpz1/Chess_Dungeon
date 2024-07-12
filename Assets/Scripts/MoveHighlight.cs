using UnityEngine;

public class MoveHighlight : MonoBehaviour
{
    private Player player;
    private Vector2Int position;

    public void Initialize(Player player, Vector2Int position)
    {
        this.player = player;
        this.position = position;
    }

    void OnMouseDown()
    {
        player.Move(position);
    }
}
