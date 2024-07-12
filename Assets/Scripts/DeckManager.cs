using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck; // 牌库
    public List<Card> hand; // 手牌
    public List<Card> discardPile; // 弃牌堆
    public int handSize = 3; // 手牌大小

    public GameObject cardButtonPrefab; // 用于显示卡牌的按钮预制件
    public Transform cardPanel; // 卡牌面板
    public Text deckCountText; // 显示牌库剩余牌数的文本组件
    public Text discardPileCountText; // 显示弃牌堆剩余牌数的文本组件

    private List<GameObject> cardButtons; // 用于追踪卡牌按钮

    void Start()
    {
        cardButtons = new List<GameObject>();
        discardPile = new List<Card>();
        InitializeDeck();
        DrawCards(handSize);
        UpdateDeckCountText(); // 初始化时更新牌堆数量显示
        UpdateDiscardPileCountText(); // 初始化时更新弃牌堆数量显示
    }

    void InitializeDeck()
    {
        // 初始化牌库
        deck = new List<Card>
        {
            new Card(CardType.Move),
            new Card(CardType.Move),
            new Card(CardType.Move),
            new Card(CardType.Attack),
            new Card(CardType.Attack)
        };

        ShuffleDeck();
        Debug.Log("Deck initialized. Count: " + deck.Count); // 添加调试日志
    }

    void ShuffleDeck()
    {
        // 洗牌算法
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        Debug.Log("Deck shuffled. Count: " + deck.Count); // 添加调试日志
    }

    void DrawCards(int number)
    {
        for (int i = 0; i < number; i++)
        {
            if (deck.Count == 0)
            {
                ReshuffleDeck();
            }

            if (deck.Count > 0)
            {
                Card card = deck[0];
                deck.RemoveAt(0);
                hand.Add(card);
                Debug.Log("Drawn card: " + card.cardType + " Deck Count: " + deck.Count); // 添加调试日志

                // 创建卡牌按钮并添加到CardPanel中
                GameObject cardButton = Instantiate(cardButtonPrefab, cardPanel);
                CardButton cardButtonScript = cardButton.GetComponent<CardButton>();
                if (cardButtonScript != null)
                {
                    cardButtonScript.Initialize(card, this);
                    cardButtons.Add(cardButton); // 追踪卡牌按钮
                }
                else
                {
                    Debug.LogError("CardButton script not found on instantiated CardButton.");
                }
            }
            UpdateDeckCountText(); // 每次抽牌后更新牌库剩余数量显示
        }
    }

    public void UseCard(Card card, GameObject cardButton)
    {
        hand.Remove(card);
        discardPile.Add(card);
        card.Use(FindObjectOfType<Player>());
        int index = cardButtons.IndexOf(cardButton);
        if (index != -1)
        {
            Destroy(cardButton);
            cardButtons.RemoveAt(index);
            DrawNewCardAt(index);
            UpdateDiscardPileCountText(); // 更新弃牌堆数量显示
        }
    }

    void DrawNewCardAt(int index)
    {
        if (deck.Count == 0)
        {
            ReshuffleDeck();
        }

        if (deck.Count > 0)
        {
            Card card = deck[0];
            deck.RemoveAt(0);
            hand.Add(card);
            Debug.Log("Drawn new card at index: " + index + " Deck Count: " + deck.Count); // 添加调试日志

            // 创建卡牌按钮并添加到CardPanel中
            GameObject cardButton = Instantiate(cardButtonPrefab, cardPanel);
            CardButton cardButtonScript = cardButton.GetComponent<CardButton>();
            if (cardButtonScript != null)
            {
                cardButtonScript.Initialize(card, this);
                cardButtons.Insert(index, cardButton); // 在原位置插入新卡牌按钮
            }
            else
            {
                Debug.LogError("CardButton script not found on instantiated CardButton.");
            }
        }
        UpdateDeckCountText(); // 更新牌库数量显示
    }

    void ReshuffleDeck()
    {
        if (discardPile.Count > 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
            Debug.Log("Deck reshuffled. Count: " + deck.Count); // 添加调试日志
        }
        UpdateDeckCountText(); // 重洗后更新牌库数量显示
    }

    void UpdateDeckCountText()
    {
        if (deckCountText != null)
        {
            deckCountText.text = "Deck Count: " + deck.Count.ToString();
            Debug.Log("Deck Count Updated: " + deck.Count); // 添加调试日志
        }
    }

    void UpdateDiscardPileCountText()
    {
        if (discardPileCountText != null)
        {
            discardPileCountText.text = "Discard Pile Count: " + discardPile.Count.ToString();
            Debug.Log("Discard Pile Count Updated: " + discardPile.Count); // 添加调试日志
        }
    }
}
