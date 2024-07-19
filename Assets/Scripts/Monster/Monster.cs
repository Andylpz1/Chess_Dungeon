using UnityEngine;

public class Monster : MonoBehaviour
{
    public int health;
    public Vector2Int position;
    public Player player;

    public virtual void Initialize(Vector2Int startPos)
    {
        position = startPos;
        player = FindObjectOfType<Player>();
        UpdatePosition();
    }

    protected void UpdatePosition()
    {
        transform.position = player.CalculateWorldPosition(position);
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (player != null)
        {
            player.AddGold(10);
        }
        Destroy(gameObject);
    }

    public virtual void MoveTowardsPlayer()
    {
        // Monster movement logic
    }

    public bool IsPositionOccupied(Vector2Int checkPosition)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            Monster otherMonster = monster.GetComponent<Monster>();
            if (otherMonster != null && otherMonster.position == checkPosition)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < player.boardSize && position.y >= 0 && position.y < player.boardSize;
    }

    public virtual GameObject GetPrefab()
    {
        return null; // Return null or a default prefab if you have one
    }
}
