using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public Text nodeText; // 可选：显示关卡名称或类型
    public string nodeType; // 节点类型，例如 "Level" 或 "Reward"
    public string targetScene; // 节点对应的关卡场景名称

    // 设置节点的信息
    public void SetNodeInfo(string type, string scene)
    {
        nodeType = type;
        targetScene = scene;

        if (nodeText != null)
        {
            nodeText.text = type; // 更新 UI 显示的文本
        }
    }

    // 点击节点的逻辑
    public void OnNodeClicked()
    {
        Debug.Log($"Node clicked: {nodeType}");
        if (!string.IsNullOrEmpty(targetScene))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Target scene not assigned.");
        }
    }
}
