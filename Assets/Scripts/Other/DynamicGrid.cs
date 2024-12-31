using UnityEngine;
using UnityEngine.UI;

public class DynamicGrid : MonoBehaviour
{
    public GameObject tilePrefab; // 单个格子的预制件
    public int gridWidth = 8; // 棋盘宽度
    public int gridHeight = 8; // 棋盘高度
    public Canvas canvas; // 引用Canvas
    public RectTransform gridContainer; // 用于容纳格子的容器

    private float tileSize; // 动态计算格子的大小

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // 确保Canvas和GridContainer引用不为空
        if (canvas == null || gridContainer == null || tilePrefab == null)
        {
            Debug.LogError("Please assign Canvas, GridContainer, and TilePrefab in the inspector.");
            return;
        }

        // 获取GridContainer的实际宽高
        RectTransform containerRect = gridContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;

        // 动态计算格子的大小（保证正方形格子）
        float cellWidth = containerWidth / gridWidth;
        float cellHeight = containerHeight / gridHeight;
        tileSize = Mathf.Min(cellWidth, cellHeight);

        // 清空容器中的已有格子
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }

        // 动态生成格子
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject tile = Instantiate(tilePrefab, gridContainer);
                RectTransform tileRect = tile.GetComponent<RectTransform>();

                // 设置格子的尺寸
                tileRect.sizeDelta = new Vector2(tileSize, tileSize);

                // 设置格子的位置
                tileRect.anchoredPosition = new Vector2(
                    x * tileSize,
                    y * tileSize
                );
            }
        }
    }

    public void UpdateGridSize(int newWidth, int newHeight)
    {
        gridWidth = newWidth;
        gridHeight = newHeight;
        GenerateGrid(); // 更新网格
    }
}
