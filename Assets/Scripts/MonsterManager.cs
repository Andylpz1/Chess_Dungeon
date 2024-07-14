using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    public GameObject slimePrefab; // 史莱姆怪物的预制件
    public GameObject warningPrefab; // 警告图案的预制件
    public int boardSize = 5;
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap

    private List<Slime> slimes = new List<Slime>();
    private List<GameObject> warnings = new List<GameObject>();

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

    public void SpawnSlime()
    {
        Vector2Int spawnPosition = GetRandomPosition();
        Vector3 worldPosition = CalculateWorldPosition(spawnPosition);
        GameObject slimeObject = Instantiate(slimePrefab, worldPosition, Quaternion.identity);
        Slime slime = slimeObject.GetComponent<Slime>();
        if (slime != null)
        {
            slime.Initialize(spawnPosition);
            slimes.Add(slime);
        }
        Debug.Log("Slime spawned at position: " + spawnPosition);
    }

    public void OnTurnEnd(int turnCount)
    {
        // 移除已被销毁的Slime对象
        slimes.RemoveAll(slime => slime == null);

        foreach (Slime slime in new List<Slime>(slimes))
        {
            if (slime != null && turnCount % slime.moveInterval == 0)
            {
                slime.MoveTowardsPlayer();
            }
        }
    }

    Vector2Int GetRandomPosition()
    {
        Vector2Int playerPosition = FindObjectOfType<Player>().position;
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int> { playerPosition };

        foreach (Slime slime in slimes)
        {
            occupiedPositions.Add(slime.position);
        }

        Vector2Int randomPosition;
        do
        {
            randomPosition = new Vector2Int(Random.Range(0, boardSize), Random.Range(0, boardSize));
        } while (occupiedPositions.Contains(randomPosition));

        return randomPosition;
    }

    public Vector3 CalculateWorldPosition(Vector2Int gridPosition)
    {
        // 计算世界坐标，考虑每个Tile的大小和Cell Gap，并加上偏移量使其居中
        float x = gridPosition.x * (cellSize.x) + (cellSize.x / 2);
        float y = gridPosition.y * (cellSize.y) + (cellSize.y / 2);
        return new Vector3(x, y, -1); // 确保 Z 轴位置在相机视野内
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

