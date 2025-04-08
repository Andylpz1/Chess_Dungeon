using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardNode : MonoBehaviour
{
    // 用于显示牌库的容器（例如一个 ScrollView 的 Content 对象）
    public Transform deckPanel;
    // 可选：卡牌显示尺寸
    public Vector2 cardSize = new Vector2(40, 50);
    // 完成按钮，点击后保存修改并关闭 deckPanel
    public Button completeButton;

    private void Start()
    {
        // 如果完成按钮存在，则添加点击监听器
        if (completeButton != null)
        {
            completeButton.onClick.AddListener(OnComplete);
        }
        else
        {
            Debug.LogWarning("请在 Inspector 中指定完成按钮！");
        }
    }

    // 当玩家点击奖励节点时触发
    private void OnMouseDown()
    {
        Debug.Log("点击奖励节点，更新 deckPanel 显示");
        // 激活 deckPanel 的 GameObject（如果其处于隐藏状态）
        if (deckPanel != null && deckPanel.gameObject != null)
        {
            deckPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("deckPanel 未赋值，请在 Inspector 中指定！");
        }
        UpdateDeckPanel();
    }

    // 更新 deckPanel，显示当前牌库中的所有卡牌
    public void UpdateDeckPanel()
    {
        // 清空 deckPanel 中现有的所有卡牌 UI
        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }

        // 从 GameManager 中获取牌库数据（确保 GameManager.Instance.playerDeck 已保存牌库数据）
        List<Card> deck = GameManager.Instance.playerDeck;
        if (deck == null)
        {
            Debug.LogError("无法获取 GameManager 中的牌库数据，请检查 playerDeck！");
            return;
        }

        // 遍历牌库，动态生成卡牌 UI
        foreach (Card card in deck)
        {
            // 创建一个新的 GameObject 用于显示卡牌
            GameObject cardUI = new GameObject("Card");
            
            // 添加 Image 组件，用于显示卡牌图片
            Image cardImage = cardUI.AddComponent<Image>();
            cardImage.sprite = card.GetSprite();
            
            // 设置 RectTransform 尺寸
            RectTransform rectTransform = cardUI.GetComponent<RectTransform>();
            rectTransform.sizeDelta = cardSize;
            
            // 将生成的卡牌 UI 作为 deckPanel 的子对象，保持相对布局
            cardUI.transform.SetParent(deckPanel, false);
            
            // 添加 Button 组件，点击时删除该卡牌
            Button cardButton = cardUI.AddComponent<Button>();
            // 指定 targetGraphic 为刚添加的 Image，确保点击事件正确响应
            cardButton.targetGraphic = cardImage;
            cardButton.onClick.AddListener(() => OnCardClicked(card));
        }
    }

    // 点击卡牌时调用，删除选中卡牌并更新显示
    public void OnCardClicked(Card selectedCard)
    {
        Debug.Log("删除选中卡牌：" + selectedCard.cardName);
        List<Card> deck = GameManager.Instance.playerDeck;
        if (deck.Contains(selectedCard))
        {
            deck.Remove(selectedCard);
            UpdateDeckPanel();
        }
        else
        {
            Debug.LogError("牌库中不存在该卡牌！");
        }
    }

    // 完成按钮的点击事件
    public void OnComplete()
    {
        Debug.Log("完成修改，保存卡组并关闭 deckPanel");
        // 如果需要在此处调用额外的存档保存逻辑，可以在这里调用 GameManager 的保存函数
        GameManager.Instance.SaveDeck();
        // 关闭 deckPanel
        if (deckPanel != null && deckPanel.gameObject != null)
        {
            deckPanel.gameObject.SetActive(false);
        }
    }
}
