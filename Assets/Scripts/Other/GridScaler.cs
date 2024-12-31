using UnityEngine;

[ExecuteAlways]
public class GridTransformScaler : MonoBehaviour
{
    public Camera mainCamera; // Reference to the Main Camera
    public int gridWidth = 8; // Number of columns in the grid
    public int gridHeight = 8; // Number of rows in the grid
    public float tileSize = 1f; // Desired size of each tile in world units

    private Transform gridTransform; // Reference to the Grid Transform

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Assign the main camera if not set
        }

        gridTransform = transform; // Cache the grid transform
        UpdateGridScale();
    }

    void Update()
    {
        UpdateGridScale();
    }

    void UpdateGridScale()
    {
        if (mainCamera == null || gridTransform == null) return;

        // Get the screen aspect ratio
        float aspectRatio = (float)Screen.width / Screen.height;

        // Calculate camera size to fit the grid correctly
        float cameraHeight = gridHeight * tileSize / 2f;
        float cameraWidth = (gridWidth * tileSize / 2f) / aspectRatio;
        mainCamera.orthographicSize = Mathf.Max(cameraHeight, cameraWidth);

        // Adjust grid scale to make tiles perfect squares
        float scaleX = tileSize / (mainCamera.orthographicSize * 2 * aspectRatio / gridWidth);
        float scaleY = tileSize / (mainCamera.orthographicSize * 2 / gridHeight);

        gridTransform.localScale = new Vector3(scaleX, scaleY, 1f);

        Debug.Log($"Updated Grid Scale: {gridTransform.localScale}, Camera Size: {mainCamera.orthographicSize}");
    }
}
