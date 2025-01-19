using UnityEngine;
using UnityEngine.EventSystems; 
using System.Collections.Generic;

public class Monster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string monsterName = "default";
    private Animator animator;
    public int health;
    public Vector2Int position;
    public Player player;
    private MonsterManager monsterManager;

    public MonsterInfoManager infoManager;

    public virtual void Initialize(Vector2Int startPos)
    {
        position = startPos;
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player object not found! Make sure a Player is present in the scene.");
            return;
        }
        monsterManager = FindObjectOfType<MonsterManager>();
        infoManager = FindObjectOfType<MonsterInfoManager>();

        UpdatePosition();
    }

    protected void UpdatePosition()
    {
        transform.position = player.CalculateWorldPosition(position);
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        // 播放受伤动画
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }

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
         if (monsterManager != null)
        {
            monsterManager.RemoveMonster(this);
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
            if (otherMonster != null && otherMonster != this && otherMonster.IsPartOfMonster(checkPosition))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsValidPosition(Vector2Int position)
    {
        foreach (Vector2Int pos in GetOccupiedPositions(position)) {
            if (position.x < 0 || position.x >= player.boardSize || position.y < 0 || position.y >= player.boardSize)
            {
                return false;
            }
        }
        return true;
    }

    public virtual GameObject GetPrefab()
    {
        return null; // Return null or a default prefab if you have one
    }

    public virtual bool IsPartOfMonster(Vector2Int position)
    {
        return this.position == position;
    }

    public virtual List<Vector2Int> GetOccupiedPositions(Vector2Int position)
    {
        return new List<Vector2Int> { position }; // Default to single-tile monster
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Pointer entered monster: {monsterName}"); // 调试用日志
        if (infoManager != null)
        {
            // 调用 MonsterInfoManager 更新信息面板
            infoManager.UpdateMonsterInfo(monsterName, health, position);
        }
    }

    // 鼠标移出事件
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Pointer exited monster: {monsterName}"); // 调试用日志
        if (infoManager != null)
        {
            // 调用 MonsterInfoManager 隐藏信息面板
            infoManager.HideMonsterInfo();
        }
    }

}
