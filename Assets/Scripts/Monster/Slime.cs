using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
        transform.position = player.CalculateWorldPosition(position);
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

        Vector2Int originalPosition = position;
        Vector2Int direction = player.position - position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 优先沿着 x 方向移动
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            possibleMoves.Add(new Vector2Int(position.x + (int)Mathf.Sign(direction.x), position.y));
            possibleMoves.Add(new Vector2Int(position.x, position.y + (int)Mathf.Sign(direction.y)));
        }
        else // 优先沿着 y 方向移动
        {
            possibleMoves.Add(new Vector2Int(position.x, position.y + (int)Mathf.Sign(direction.y)));
            possibleMoves.Add(new Vector2Int(position.x + (int)Mathf.Sign(direction.x), position.y));
        }

        // 尝试每一个可能的移动方向，直到找到一个未被占据的位置
        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPosition(move))
            {
                position = move;
                break;
            }
        }

        UpdatePosition();

        // 检测是否接触到玩家
        if (position == player.position)
        {
            Debug.Log("Player touched by Slime. Game Over.");
        // 在此处添加游戏结束逻辑
        }
    }

    // 检查位置是否被其他怪物占据
    private bool IsPositionOccupied(Vector2Int checkPosition)
    {   
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Slime slime = monster.GetComponent<Slime>();
            if (slime != null && slime.position == checkPosition)
            {
                return true;
            }
        }
        return false;
    }

    // 检查位置是否有效
    private bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < player.boardSize && position.y >= 0 && position.y < player.boardSize;
    }


}
