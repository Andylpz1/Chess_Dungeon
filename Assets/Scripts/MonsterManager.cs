using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    public GameObject slimePrefab; // 史莱姆怪物的预制件
    public GameObject warningPrefab; // 警告图案的预制件
    public int boardSize = 6;
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap

    private List<Monster> monsters = new List<Monster>();
    private List<GameObject> warnings = new List<GameObject>();

    public Player player; // 玩家对象

    void Awake()
    {
        // Initialize the player in Awake to ensure it is set before Start
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player object not found!");
        }
    }

    void Start()
    {
        
        SpawnWarning();
    }

    public void SpawnWarning()
    {
        //Vector2Int warningPosition = GetRandomPosition();
        //Vector3 worldPosition = CalculateWorldPosition(warningPosition);
        //GameObject warningObject = Instantiate(warningPrefab, worldPosition, Quaternion.identity);
        //warnings.Add(warningObject);
        //Debug.Log("Warning spawned at position: " + warningPosition);
    }

    public void SpawnMonster(Monster monsterType)
    {
        Vector2Int spawnPosition = GetRandomPosition(monsterType);
        Vector3 worldPosition = player.CalculateWorldPosition(spawnPosition);
        GameObject monsterObject = Instantiate(monsterType.GetPrefab(), worldPosition, Quaternion.identity);
        Monster monster = monsterObject.GetComponent<Monster>();
        if (monster != null)
        {
            monster.Initialize(spawnPosition);
            monsters.Add(monster);
        }
        Debug.Log($"{monsterType.GetType().Name} spawned at position: " + spawnPosition);
    }

    public void MoveMonsters()
    {
        StartCoroutine(MoveMonstersSequentially());
    }

    private IEnumerator MoveMonstersSequentially()
    {
        List<Monster> monstersCopy = new List<Monster>(monsters);
        foreach (Monster monster in monstersCopy)
        {
            if (monster != null)
            {
                monster.MoveTowardsPlayer();
                yield return new WaitForSeconds(0.5f); // 延迟0.5秒
            }
        }
        yield return new WaitForSeconds(0.5f); // 在所有怪物移动后延迟0.5秒
        //生成新的怪物
        SpawnMonster(new Slime());
        SpawnMonster(new Bat());
        SpawnMonster(new Hound());
    }

    public void OnTurnEnd(int turnCount)
    {
        // 移除已被销毁的Monster对象
        monsters.RemoveAll(monster => monster == null);

        if (turnCount % 1 == 0)
        {
            MoveMonsters();
            Debug.Log("Monsters move.");
        }
    }

    Vector2Int GetRandomPosition(Monster monsterType)
    {
        Vector2Int playerPosition = player.position;
        Debug.Log("yaleyale" + playerPosition);
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int> { playerPosition };
        // Add all occupied positions
        foreach (Monster monster in monsters)
        {
            occupiedPositions.UnionWith(monster.GetOccupiedPositions(monster.position));
        }

        Vector2Int restrictedPosition = new Vector2Int(3, 3); // 永远不会生成的位置

        Vector2Int randomPosition;
        List<Vector2Int> monsterParts;

        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
            monsterParts = monsterType.GetOccupiedPositions(randomPosition);
        } while (occupiedPositions.Overlaps(monsterParts) || randomPosition == restrictedPosition || !AreAllPositionsValid(monsterParts) || monsterParts.Contains(playerPosition));

        return randomPosition;
    }


    bool AreAllPositionsValid(List<Vector2Int> positions)
    {
        foreach (Vector2Int pos in positions)
        {
            if (pos.x < 0 || pos.x >= boardSize || pos.y < 0 || pos.y >= boardSize)
            {
                return false;
            }
        }
        return true;
    }

    public int GetMonsterCount()
    {
        return monsters.Count;
    }

    void ClearWarnings()
    {
        //foreach (GameObject warning in warnings)
        //{
            //Destroy(warning);
        //}
        //warnings.Clear();
    }
}
