using UnityEngine;

[ExecuteAlways]
public class TilemapScaler : MonoBehaviour
{
    public Camera mainCamera; // 主相机
    public Grid grid;         // 绑定的Grid对象
    public int gridWidth = 8; // Tilemap的宽度
    public int gridHeight = 8; // Tilemap的高度

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (grid == null)
        {
            grid = GetComponent<Grid>();
        }

        UpdateCameraAndTilemapScale();
    }

    void Update()
    {
        // 如果在编辑器中运行，可以实时更新
        if (!Application.isPlaying)
        {
            UpdateCameraAndTilemapScale();
        }
    }

    private void UpdateCameraAndTilemapScale()
    {
        if (mainCamera == null || grid == null)
        {
            Debug.LogError("Main Camera or Grid is not assigned!");
            return;
        }

        // 计算屏幕宽高比
        float aspectRatio = (float)Screen.width / Screen.height;

        // 动态设置相机大小以适配Tilemap
        float targetOrthographicSize = Mathf.Max(gridHeight / 2f, (gridWidth / 2f) / aspectRatio);
        mainCamera.orthographicSize = targetOrthographicSize;

        // 动态调整Grid的Cell尺寸
        float cellSizeX = (mainCamera.orthographicSize * 2 * aspectRatio) / gridWidth;
        float cellSizeY = (mainCamera.orthographicSize * 2) / gridHeight;

        grid.cellSize = new Vector3(cellSizeX, cellSizeY, 1);

        Debug.Log($"Updated Grid Cell Size: {grid.cellSize}, Camera Size: {mainCamera.orthographicSize}");
    }
}
