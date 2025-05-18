using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeNode : MonoBehaviour
{
    // 用于显示牌库的容器（例如一个 ScrollView 的 Content 对象）
    public Transform deckPanel;
    // 卡牌显示尺寸
    public Vector2 cardSize = new Vector2(40, 50);
    // 用于显示升级选项的面板（初始隐藏）
    public Transform upgradePanel;
    // 升级按钮预制体（预制体内需包含 Text 和 Button 组件）
    public GameObject upgradeButtonPrefab;
    // 完成按钮，点击后保存修改并关闭所有面板
    public Button completeButton;
    // 关卡节点所在的 Canvas 或父物体（打开 RewardNode 时需要隐藏）
    public GameObject nodeCanvas;

    // 升级按钮尺寸
    public Vector2 upgradeButtonSize = new Vector2(100, 40);

    // 当前选中的卡牌（待升级）
    private Card selectedCard;

    private void Start()
    {
        // 完成按钮添加点击事件
        if (completeButton != null)
        {
            completeButton.onClick.AddListener(OnComplete);
            completeButton.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("请在 Inspector 中指定 completeButton！");
        }

        // 初始隐藏 deckPanel 与 upgradePanel
        if (deckPanel != null)
            deckPanel.gameObject.SetActive(false);
        if (upgradePanel != null)
            upgradePanel.gameObject.SetActive(false);
    }

    // 当玩家点击 RewardNode 时触发
    private void OnMouseDown()
    {
        Debug.Log("点击 RewardNode，显示牌库");
        // 隐藏关卡节点所在的 nodeCanvas
        if (nodeCanvas != null)
            nodeCanvas.SetActive(false);
        else
            Debug.LogWarning("请在 Inspector 中指定 nodeCanvas！");

        // 显示 deckPanel 并更新牌库显示
        if (deckPanel != null)
            deckPanel.gameObject.SetActive(true);
        else
            Debug.LogError("deckPanel 未指定，请在 Inspector 中设置！");
        if (completeButton != null)
            completeButton.gameObject.SetActive(true);
        UpdateDeckPanel();
    }

    // 更新 deckPanel，展示当前牌库中所有卡牌
    public void UpdateDeckPanel()
    {
        // 清空 deckPanel 中的所有子对象
        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }

        List<Card> deck = GameManager.Instance.playerDeck;
        if (deck == null)
        {
            Debug.LogError("无法获取 GameManager 中的牌库数据，请检查 playerDeck！");
            return;
        }

        // 对每张卡牌创建一个按钮（这里用代码动态创建，实际项目中可用预制体）
        foreach (Card card in deck)
        {
            GameObject cardUI = new GameObject("Card");
            Image cardImage = cardUI.AddComponent<Image>();
            cardImage.sprite = card.GetSprite();

            RectTransform rectTransform = cardUI.GetComponent<RectTransform>();
            rectTransform.sizeDelta = cardSize;

            cardUI.transform.SetParent(deckPanel, false);

            Button cardButton = cardUI.AddComponent<Button>();
            cardButton.targetGraphic = cardImage;
            // 点击卡牌后不再直接删除，而是展示升级选项
            var cardCopy = card;    
            cardButton.onClick.AddListener(() => OnCardClicked(cardCopy));
        }
    }

    // 当点击某张卡牌时，保存该卡牌为选中项，并展示升级选项
    public void OnCardClicked(Card card)
    {
        selectedCard = card;
        ShowUpgradeOptions();
    }

    // 展示升级选项
    public void ShowUpgradeOptions()
    {
        if (upgradePanel == null)
        {
            Debug.LogError("upgradePanel 未指定，请在 Inspector 中设置！");
            return;
        }
        // 清空升级选项面板
        foreach (Transform child in upgradePanel)
        {
            Destroy(child.gameObject);
        }
        upgradePanel.gameObject.SetActive(true);

        // 遍历 CardUpgrade 枚举，生成按钮
        foreach (CardUpgrade upgrade in selectedCard.UpgradeOptions)
        {
            // 已经拥有的升级就不再生成按钮（可选）
             if (selectedCard.HasUpgrade(upgrade))
                continue;

            GameObject upgradeBtnObj = Instantiate(upgradeButtonPrefab, upgradePanel);
            upgradeBtnObj.name = "Upgrade_" + upgrade.ToString();

            // 设置按钮文本
            Text btnText = upgradeBtnObj.GetComponentInChildren<Text>();
            if (btnText != null)
                btnText.text = upgrade.ToString();

            RectTransform rt = upgradeBtnObj.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = upgradeButtonSize;

            Button btn = upgradeBtnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnUpgradeSelected(upgrade));
            }
        }
    }

    // 当玩家点击升级选项时调用：对选中卡牌升级
    public void OnUpgradeSelected(CardUpgrade upgrade)
    {
        if (selectedCard != null)
        {
            // 对选中卡牌执行升级操作（修改内部状态，例如添加升级记录）
            selectedCard.AddUpgrade(upgrade);
            Debug.Log($"卡牌 {selectedCard.cardName} 升级成功：{upgrade}");
        }
        else
        {
            Debug.LogError("没有选中的卡牌！");
        }
        // 隐藏升级选项面板
        if (upgradePanel != null)
            upgradePanel.gameObject.SetActive(false);
        // 刷新牌组显示（以便展示升级后的效果）
        UpdateDeckPanel();
    }




    // 完成按钮点击后，保存牌组并关闭所有面板，恢复关卡节点
    public void OnComplete()
    {
        Debug.Log("完成升级，保存牌组并关闭界面");
        // 调用 GameManager 的 SaveDeck 方法保存当前牌组（假设该方法已经实现）
        GameManager.Instance.SaveDeck();
        // 隐藏 deckPanel 与 upgradePanel
        if (deckPanel != null)
            deckPanel.gameObject.SetActive(false);
        if (upgradePanel != null)
            upgradePanel.gameObject.SetActive(false);
        // 恢复关卡节点
        if (nodeCanvas != null)
            nodeCanvas.SetActive(true);
        if (completeButton != null)
            completeButton.gameObject.SetActive(false);
    }
}
