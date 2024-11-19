using UnityEngine;
using System.Collections.Generic;

public class NodeManager : MonoBehaviour
{
    public GameObject nodePrefab; // 节点的 prefab
    public Transform parent; // 节点的父容器
    public float verticalSpacing = 100f; // 垂直间距
    public int totalLevels = 5; // 总层数（包括 root 和 final）
    public int minNodesPerLevel = 2; // 每层最少节点数
    public int maxNodesPerLevel = 4; // 每层最多节点数
    public float minHorizontalSpacing = 150f; // 两个节点之间的最小水平距离

    private List<List<Vector2>> levels = new List<List<Vector2>>(); // 存储每层节点的位置

    void Start()
    {
        GenerateTree();
    }

    void GenerateTree()
    {
        // 1. 生成起始节点（Root）
        Vector2 rootPosition = new Vector2(0, 500);
        levels.Add(new List<Vector2> { rootPosition });
        CreateNode(rootPosition, "Start");

        // 2. 生成中间层
        for (int i = 1; i < totalLevels - 1; i++)
        {
            GenerateLevel(i);
        }

        // 3. 生成终点节点（Final）
        //Vector2 finalPosition = new Vector2(0, -verticalSpacing * (totalLevels - 1));
        Vector2 finalPosition = new Vector2(0, -500);
        levels.Add(new List<Vector2> { finalPosition });
        CreateNode(finalPosition, "Final");

        // 4. 连接节点并确保完整路径
        ConnectNodes();
    }

    void GenerateLevel(int levelIndex)
    {
        List<Vector2> currentLevel = new List<Vector2>();

        // 随机生成本层的节点数量
        int nodeCount = Random.Range(minNodesPerLevel, maxNodesPerLevel + 1);

        for (int i = 0; i < nodeCount; i++)
        {
            // 尝试生成一个符合水平间距要求的位置
            Vector2 position;
            int attempts = 0;
            do
            {
                float xPosition = Random.Range(-700f, 700f); // 随机水平位置
                float yPosition = 500 - levelIndex * verticalSpacing + Random.Range(0f, 50f); // 添加 y 偏移
                position = new Vector2(xPosition, yPosition);
                attempts++;
            } while (!IsValidPosition(position, currentLevel) && attempts < 10);

            currentLevel.Add(position);
            CreateNode(position, "Battle"); // 默认类型为 "Battle"
        }

        levels.Add(currentLevel);
    }

    bool IsValidPosition(Vector2 position, List<Vector2> currentLevel)
    {
        foreach (Vector2 existingPosition in currentLevel)
        {
            if (Mathf.Abs(existingPosition.x - position.x) < minHorizontalSpacing)
            {
                return false; // 水平距离不足
            }
        }
        return true;
    }

    void CreateNode(Vector2 position, string nodeType)
    {
        GameObject node = Instantiate(nodePrefab, parent);
        RectTransform rectTransform = node.GetComponent<RectTransform>();

        // 设置节点缩放
        rectTransform.localScale = new Vector3(50f, 50f, 1f);

        // 设置节点位置
        rectTransform.anchoredPosition = position;

        // 配置节点信息
        NodeUI nodeUI = node.GetComponent<NodeUI>();
        if (nodeUI != null)
        {
            nodeUI.SetNodeInfo(nodeType, "LevelScene");
        }
    }

    void ConnectNodes()
    {
        for (int i = levels.Count - 1; i > 0; i--) // 从底层到顶层处理
        {
            List<Vector2> currentLevel = levels[i];
            List<Vector2> previousLevel = levels[i - 1];

            // 按水平位置排序，减少交叉
            currentLevel.Sort((a, b) => a.x.CompareTo(b.x));
            previousLevel.Sort((a, b) => a.x.CompareTo(b.x));

            HashSet<Vector2> connectedNodes = new HashSet<Vector2>();

            // 确保每个节点至少有一个父节点
            foreach (Vector2 position in currentLevel)
            {
                // 找到最近的父节点
                Vector2 closestParent = FindClosestParent(position, previousLevel);
                DrawConnection(position, closestParent);

                // 标记父节点已被连接
                connectedNodes.Add(closestParent);
            }

            // 确保上一层每个节点有至少一个子节点
            foreach (Vector2 parentNode in previousLevel)
            {
                if (!connectedNodes.Contains(parentNode))
                {
                    // 找到最近的子节点
                    Vector2 closestChild = FindClosestChild(parentNode, currentLevel);
                    DrawConnection(closestChild, parentNode);
                }
            }
        }
    }

    Vector2 FindClosestParent(Vector2 child, List<Vector2> parents)
    {
        Vector2 closest = parents[0];
        float minDistance = Mathf.Abs(child.x - closest.x);

        foreach (Vector2 parent in parents)
        {
            float distance = Mathf.Abs(child.x - parent.x);
            if (distance < minDistance)
            {
                closest = parent;
                minDistance = distance;
            }
        }

        return closest;
    }

    Vector2 FindClosestChild(Vector2 parent, List<Vector2> children)
    {
        Vector2 closest = children[0];
        float minDistance = Mathf.Abs(parent.x - closest.x);

        foreach (Vector2 child in children)
        {
            float distance = Mathf.Abs(parent.x - child.x);
            if (distance < minDistance)
            {
                closest = child;
                minDistance = distance;
            }
        }

        return closest;
    }

    void DrawConnection(Vector2 from, Vector2 to)
    {
        GameObject line = new GameObject("Connection");
        line.transform.SetParent(parent);

        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        // 设置连接线位置
        lineRenderer.SetPosition(0, parent.TransformPoint(from));
        lineRenderer.SetPosition(1, parent.TransformPoint(to));

        // 设置线条宽度
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // 设置材质和颜色
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }
}
