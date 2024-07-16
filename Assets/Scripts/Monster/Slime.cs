using UnityEngine;

public class Slime : MonoBehaviour
{
    public int health = 1;
    public Vector2Int position;
    public Player player;
    public int moveInterval = 1; // 每隔一个回合移动一次

    public void Initialize(Vector2Int startPos)
    {
        position = startPos;
        player = FindObjectOfType<Player>();
        UpdatePosition();
    }

    void UpdatePosition()
    {
        // 使用MonsterManager的计算方法确保位置正确
        MonsterManager manager = FindObjectOfType<MonsterManager>();
        transform.position = manager.CalculateWorldPosition(position);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.AddGold(10);
        }
        Destroy(gameObject);

    }
    public void MoveTowardsPlayer()
    {
        if (player == null) return;

        Vector2Int direction = player.position - position;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            position.x += (int)Mathf.Sign(direction.x);
        }
        else
        {
            position.y += (int)Mathf.Sign(direction.y);
        }

        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player touched by Slime. Game Over.");
            // 在此处添加游戏结束逻辑
        }
    }
}
