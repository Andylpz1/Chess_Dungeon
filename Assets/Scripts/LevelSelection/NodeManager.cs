using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public GameObject nodePrefab; // 节点的 prefab
    public Transform parent; // 节点的父容器
    public float horizontalSpacing = 200f; // 水平间距
    public float verticalSpacing = 150f; // 垂直间距
    public int depth = 5; // 二叉树的深度

    void Start()
    {
        GenerateBinaryTree(depth);
    }

    void GenerateBinaryTree(int depth)
    {
        // 起始位置（根节点的位置）
        Vector2 rootPosition = new Vector2(0, 300);

        // 递归生成二叉树
        CreateNode(rootPosition, depth, 0);
    }

    void CreateNode(Vector2 position, int currentDepth, int index)
    {
        if (currentDepth <= 0) return;

        // 创建节点
        GameObject node = Instantiate(nodePrefab, parent);
        RectTransform rectTransform = node.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(50f, 50f, 1f);
        rectTransform.anchoredPosition = position;

        // 配置节点信息
        NodeUI nodeUI = node.GetComponent<NodeUI>();
        if (nodeUI != null)
        {
            nodeUI.SetNodeInfo($"Level {index}", "LevelScene");
        }

        // 计算固定的水平间距
        float levelWidth = horizontalSpacing * Mathf.Pow(2, currentDepth - 1); // 当前层的总宽度
        float childSpacing = levelWidth / Mathf.Pow(2, currentDepth);         // 子节点之间的间距

        // 计算左右子节点位置
        Vector2 leftChildPos = position + new Vector2(-childSpacing / 2, -verticalSpacing);
        Vector2 rightChildPos = position + new Vector2(childSpacing / 2, -verticalSpacing);

        // 递归生成左右子节点
        CreateNode(leftChildPos, currentDepth - 1, index * 2 + 1);
        CreateNode(rightChildPos, currentDepth - 1, index * 2 + 2);

        // 创建连接线（可选）
        if (currentDepth > 1)
        {
        DrawConnection(position, leftChildPos);
        DrawConnection(position, rightChildPos);
    }
}



    void DrawConnection(Vector2 from, Vector2 to)
    {
        GameObject line = new GameObject("Connection");
        line.transform.SetParent(parent);

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, parent.TransformPoint(from));
        lineRenderer.SetPosition(1, parent.TransformPoint(to));

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }
}
