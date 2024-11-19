using UnityEngine;

[System.Serializable]
public class NodeData
{
    public string nodeType; // 节点类型，例如 "Level" 或 "Reward"
    public string targetScene; // 目标场景名称
    public Vector2 position; // 节点在 UI 中的位置
    public NodeData[] connectedNodes; // 当前节点连接的下层节点
}
