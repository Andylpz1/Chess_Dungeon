using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel; // 商店面板
    public List<Card> availableCards; // 可购买的卡牌

    // 预先设置好的卡牌UI和购买按钮
    public Image cardImage1;
    public Image cardImage2;
    public Image cardImage3;
    public Button buyButton1;
    public Button buyButton2;
    public Button buyButton3;

    public Player player; // 玩家对象

    private void Start()
    {
        Debug.Log("ShopManager script has started."); // 调试日志
        InitializeAvailableCards(); // 初始化可购买的卡牌
        DisplayAvailableCards();
    }

    void InitializeAvailableCards()
    {
        availableCards = new List<Card>
        {
            new PawnCard(),
            new KnightCard(),
            new BishopCard(),
            new AttackCard(),
        };
        Debug.Log("Available cards initialized: " + availableCards.Count); // 打印卡牌数量
    }

    void DisplayAvailableCards()
    {
        Debug.Log("DisplayAvailableCards called"); // 调试日志

        if (availableCards.Count >= 3)
        {
            // 设置第一张卡牌
            cardImage1.sprite = availableCards[0].GetSprite();
            buyButton1.GetComponentInChildren<Text>().text = "Buy (" + availableCards[0].cost + " gold)";
            buyButton1.onClick.AddListener(() => BuyCard(availableCards[0], cardImage1, buyButton1));

            // 设置第二张卡牌
            cardImage2.sprite = availableCards[1].GetSprite();
            buyButton2.GetComponentInChildren<Text>().text = "Buy (" + availableCards[1].cost + " gold)";
            buyButton2.onClick.AddListener(() => BuyCard(availableCards[1], cardImage2, buyButton2));

            // 设置第三张卡牌
            cardImage3.sprite = availableCards[2].GetSprite();
            buyButton3.GetComponentInChildren<Text>().text = "Buy (" + availableCards[2].cost + " gold)";
            buyButton3.onClick.AddListener(() => BuyCard(availableCards[3], cardImage3, buyButton3));
        }
        else
        {
            Debug.LogError("Not enough available cards to display in the shop.");
        }
    }

    void BuyCard(Card card, Image cardImage, Button buyButton)
    {
        if (player.gold >= card.cost)
        {
            player.gold -= card.cost;
            player.UpdateGoldText();
            player.deckManager.deck.Add(card); // 将购买的卡牌添加到玩家的牌库
            player.deckManager.UpdateDeckCountText();
            player.deckManager.UpdateDeckPanel();
            Debug.Log("Bought card: " + card.Id);

            // 获取一张新的随机卡牌并更新显示
            Card newCard = GetRandomCard();
            if (newCard != null)
            {
                cardImage.sprite = newCard.GetSprite();
                buyButton.GetComponentInChildren<Text>().text = "Buy (" + newCard.cost + " gold)";
                buyButton.onClick.RemoveAllListeners(); // 移除旧的监听器
                buyButton.onClick.AddListener(() => BuyCard(newCard, cardImage, buyButton));
            }
        }
        else
        {
            Debug.Log("Not enough gold to buy this card.");
        }
    }

    Card GetRandomCard()
    {
        if (availableCards.Count > 0)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            return availableCards[randomIndex];
        }
        return null;
    }
}