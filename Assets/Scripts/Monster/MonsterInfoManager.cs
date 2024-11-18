using UnityEngine;
using UnityEngine.UI;

public class MonsterInfoManager : MonoBehaviour
{
    public GameObject MonsterInfoPanel; // 显示怪物信息的面板
    public Text MonsterNameText;        // 用于显示怪物名称的文本
    public Text MonsterHealthText;     // 用于显示怪物血量的文本
    public Text MonsterPositionText;   // 用于显示怪物位置的文本

    // 更新怪物信息的面板内容
    public void UpdateMonsterInfo(string name, int health, Vector2Int position)
    {
        if (MonsterInfoPanel != null)
        {
            MonsterInfoPanel.SetActive(true); // 确保面板是可见的
            MonsterNameText.text = $"Name: {name}";
            MonsterHealthText.text = $"Health: {health}";
            MonsterPositionText.text = $"Position: {position.x}, {position.y}";
        }
        else
        {
            Debug.LogWarning("MonsterInfoPanel is not assigned in the Inspector.");
        }
    }

    // 隐藏怪物信息的面板
    public void HideMonsterInfo()
    {
        if (MonsterInfoPanel != null)
        {
            MonsterInfoPanel.SetActive(false); // 隐藏面板
        }
        else
        {
            Debug.LogWarning("MonsterInfoPanel is not assigned in the Inspector.");
        }
    }
}
