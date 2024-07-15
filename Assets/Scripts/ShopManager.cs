using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public GameObject cardButtonPrefab; // 卡牌按钮预制件
    public Transform shopPanel; // 商店面板
    public DeckManager deckManager; // 牌库管理器
    public Player player; // 玩家
    public List<Card> availableCards; // 可购买的卡牌列表

    void Start()
    {
        PopulateShop();
    }

    void PopulateShop()
    {
        foreach (Card card in availableCards)
        {
            GameObject cardButton = Instantiate(cardButtonPrefab, shopPanel);
            CardButton cardButtonScript = cardButton.GetComponent<CardButton>();
            if (cardButtonScript != null)
            {
                cardButtonScript.Initialize(card, null);
                cardButton.GetComponent<Button>().onClick.AddListener(() => BuyCard(card));
            }
        }
    }

    public void BuyCard(Card card)
    {
        int cardCost = 10; // 设置卡牌的购买价格
        if (player.gold >= cardCost)
        {
            player.AddGold(-cardCost);
            //deckManager.AddCardToDeck(card);
        }
    }

    public void RemoveCard(Card card)
    {
        int cardCost = 10; // 设置删除卡牌的价格
        if (player.gold >= cardCost)
        {
            player.AddGold(-cardCost);
            //deckManager.RemoveCardFromDeck(card);
        }
    }
}
