using UnityEngine;
using System.Collections;
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

    public Player player; // 玩家对象

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
        Vector3 worldPosition = player.CalculateWorldPosition(spawnPosition);
        GameObject slimeObject = Instantiate(slimePrefab, worldPosition, Quaternion.identity);
        Slime slime = slimeObject.GetComponent<Slime>();
        if (slime != null)
        {
            slime.Initialize(spawnPosition);
            slimes.Add(slime);
        }
        Debug.Log("Slime spawned at position: " + spawnPosition);
    }

    public void MoveSlimes()
    {
        StartCoroutine(MoveSlimesSequentially());
    }

    private IEnumerator MoveSlimesSequentially()
    {
        List<Slime> slimesCopy = new List<Slime>(slimes);
        foreach (Slime slime in slimesCopy)
        {
            if (slime != null)
            {
                slime.MoveTowardsPlayer();
                yield return new WaitForSeconds(0.5f); // 延迟0.5秒
            }
        }
        yield return new WaitForSeconds(0.5f); // 在所有史莱姆移动后延迟0.5秒
        //生成一只新的史莱姆
        SpawnSlime();
    }

    public void OnTurnEnd(int turnCount)
    {
        // 移除已被销毁的Slime对象
        slimes.RemoveAll(slime => slime == null);

        if (turnCount % 1 == 0)
        {
            MoveSlimes();
            Debug.Log("Slimes move.");
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


    public int GetSlimeCount()
    {
        return slimes.Count;
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

