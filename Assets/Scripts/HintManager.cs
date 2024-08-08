using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    public GameObject hintPanel;
    public Text hintText;
    public Image hintImage; // 用于显示卡片图片的Image组件
    public Button fullScreenButton; // 全屏按钮

    void Start()
    {
        if (hintPanel == null || hintText == null || hintImage == null || fullScreenButton == null)
        {
            Debug.LogError("HintPanel, HintText, HintImage, or FullScreenButton is not assigned in the Inspector.");
        }
        else
        {
            hintPanel.SetActive(false); // 默认隐藏提示面板
            fullScreenButton.gameObject.SetActive(false); // 默认隐藏全屏按钮
            fullScreenButton.onClick.AddListener(OnFullScreenButtonClick); // 添加点击事件监听器
        }
    }

    public void ShowHint(string message, Vector3 position, Sprite cardSprite=null)
    {
        if (hintPanel != null && hintText != null && hintImage != null)
        {
            hintPanel.SetActive(true);
            hintText.text = message;
            hintImage.sprite = cardSprite; // 设置hintImage的sprite
            fullScreenButton.gameObject.SetActive(true); // 显示全屏按钮
        }
    }

    public void HideHint()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
            fullScreenButton.gameObject.SetActive(false); // 隐藏全屏按钮
        }
    }

    public bool IsHintVisible()
    {
        return hintPanel != null && hintPanel.activeSelf;
    }

    private void OnFullScreenButtonClick()
    {
        HideHint();
    }
}
