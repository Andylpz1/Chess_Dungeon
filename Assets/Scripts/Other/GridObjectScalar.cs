using UnityEngine;

public class GridObjectScaler : MonoBehaviour
{
    public Transform gridTransform; // 引用Grid的Transform

    private Vector3 initialScale;

    void Start()
    {
        if (gridTransform == null)
        {
            Debug.LogError("Grid Transform is not assigned!");
            return;
        }

        initialScale = transform.localScale / gridTransform.localScale.x; // 计算初始缩放比例
    }

    void Update()
    {
        if (gridTransform != null)
        {
            // 根据Grid的缩放同步调整
            transform.localScale = initialScale * gridTransform.localScale.x;
        }
    }
}
