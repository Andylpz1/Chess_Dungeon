using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2Int position; // 棋子在棋盘上的位置
    public int boardSize = 5;   // 棋盘大小
    public GameObject moveHighlightPrefab; // 用于显示可移动位置的预制件
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0.5f, 0.5f, 0); // Cell Gap

    private GameObject[] moveHighlights;

    void Start()
    {
        position = new Vector2Int(boardSize / 2, boardSize / 2); // 初始化棋子位置到棋盘中央
        UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = CalculateWorldPosition(position); // 更新棋子在场景中的位置
    }

    void OnMouseDown()
    {
        ShowMoveOptions();
    }

    public void ShowMoveOptions()
    {
        ClearMoveHighlights();

        moveHighlights = new GameObject[4];
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int newPosition = position + directions[i];
            if (IsValidPosition(newPosition))
            {
                HighlightPosition(newPosition, i);
            }
        }
    }

    public void ShowAttackOptions()
    {
        ClearMoveHighlights();

        // 示例代码，显示周围所有位置为攻击选项
        moveHighlights = new GameObject[4];
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int newPosition = position + directions[i];
            if (IsValidPosition(newPosition))
            {
                HighlightPosition(newPosition, i);
            }
        }
    }

    void HighlightPosition(Vector2Int newPosition, int index)
    {
        Vector3 highlightPosition = CalculateWorldPosition(newPosition);
        GameObject highlight = Instantiate(moveHighlightPrefab, highlightPosition, Quaternion.identity);
        highlight.GetComponent<MoveHighlight>().Initialize(this, newPosition);
        moveHighlights[index] = highlight;
    }

    bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    public void Move(Vector2Int newPosition)
    {
        position = newPosition;
        UpdatePosition();
        ClearMoveHighlights();
    }

    void ClearMoveHighlights()
    {
        if (moveHighlights != null)
        {
            foreach (var highlight in moveHighlights)
            {
                Destroy(highlight);
            }
        }
    }

    Vector3 CalculateWorldPosition(Vector2Int gridPosition)
    {
        // 计算世界坐标，考虑每个Tile的大小和Cell Gap，并加上偏移量使其居中
        float x = gridPosition.x * (cellSize.x + cellGap.x) + (cellSize.x / 2);
        float y = gridPosition.y * (cellSize.y + cellGap.y) + (cellSize.y / 2);
        return new Vector3(x, y, 0);
    }
}
