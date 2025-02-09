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
    private List<GameObject> highlightInstances = new List<GameObject>();
    public GameObject highlightPrefab;  // 在 Inspector 中拖入 Highlight Prefab
    
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

    public virtual List<Vector2Int> CalculatePossibleMoves()
    {
        // 默认实现：简单地计算怪物周围的 1 格可移动位置（可以根据需要定制复杂逻辑）
        List<Vector2Int> possibleMoves = new List<Vector2Int>
        {
            position + new Vector2Int(1, 0),
            position + new Vector2Int(-1, 0),
            position + new Vector2Int(0, 1),
            position + new Vector2Int(0, -1)
        };

        // 过滤掉无效位置或被占据的位置
        possibleMoves.RemoveAll(pos => !IsValidPosition(pos) || IsPositionOccupied(pos));
        return possibleMoves;
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
            if (position.x < 0 || position.x >= player.boardSize || position.y < 0 || position.y >= player.boardSize )
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
        HighlightPath();
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
        ClearHighlight();
    }

    public virtual void HighlightPath()
    {
        // 清除之前的高亮
        ClearHighlight();

        // 获取合法的移动路径
        List<Vector2Int> possibleMoves = CalculatePossibleMoves();

        // 在每个合法位置生成高亮对象
        foreach (Vector2Int move in possibleMoves)
        {
            Vector3 worldPos = player.CalculateWorldPosition(move);
            GameObject highlightInstance = Instantiate(highlightPrefab, worldPos, Quaternion.identity);
            highlightInstances.Add(highlightInstance);
        }
    }

    public void ClearHighlight()
    {
        foreach (GameObject highlight in highlightInstances)
        {
            Destroy(highlight);
        }
        highlightInstances.Clear();
    }

}
