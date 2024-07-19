using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck; // 牌库
    public List<Card> hand; // 手牌
    public List<Card> discardPile; // 弃牌堆
    public int handSize = 5; // 手牌大小

    public Transform cardPanel; // 卡牌面板
    public Transform deckPanel; // 卡组面板，用于显示卡组中卡牌的图片

    public Text deckCountText; // 显示牌库剩余牌数的文本组件
    public Text discardPileCountText; // 显示弃牌堆剩余牌数的文本组件
    public TurnManager turnManager; // 回合管理器

    public Text deletePopupMessage; // 弹窗中的提示信息
    public Button confirmDeleteButton; // 弹窗中的确认按钮
    public Button cancelDeleteButton; // 弹窗中的取消按钮
    public GameObject deletePopup; // 删除卡牌的弹窗
    private List<GameObject> cardButtons; // 用于追踪卡牌按钮
    private Card cardToDelete; // 要删除的卡牌
    public Player player; // 玩家对象

    void Start()
    {
        cardButtons = new List<GameObject>();
        discardPile = new List<Card>();
        InitializeDeck();
        DrawCards(handSize);
        UpdateDeckCountText(); // 初始化时更新牌堆数量显示
        UpdateDiscardPileCountText(); // 初始化时更新弃牌堆数量显示
        UpdateDeckPanel(); // 初始化时更新卡组显示

        if (deletePopup != null)
        {
            deletePopup.SetActive(false); // 初始时隐藏删除弹窗
            confirmDeleteButton.onClick.AddListener(ConfirmDeleteCard);
            cancelDeleteButton.onClick.AddListener(CancelDeleteCard);
        }
        else
        {
            Debug.LogError("Delete popup is not assigned in the Inspector.");
        }

        // Ensure player is assigned
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (player == null)
        {
            Debug.LogError("Player is not assigned and could not be found in the scene.");
        }
    }

    void InitializeDeck()
    {
        // 初始化牌库
        deck = new List<Card>
        {
            new PawnCard(),
            new PawnCard(),
            new PawnCard(),
            new PawnCard(),
            new PawnCard(),
            new SwordCard(),
            new SwordCard(),
            new SwordCard()
        };

        ShuffleDeck();
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
    }

    public void DrawCards(int number)
    {
        StartCoroutine(DrawCardsCoroutine(number));
    }

    private IEnumerator DrawCardsCoroutine(int number)
    {
        for (int i = 0; i < number; i++)
        {
            if (deck.Count == 0)
            {
                ReshuffleDeck();
            }

            if (deck.Count > 0)
            {
                // 随机抓牌
                ShuffleDeck();
                UpdateDeckPanel();
                Card card = deck[0];
                deck.RemoveAt(0);
                hand.Add(card);

                // 创建卡牌按钮并添加到CardPanel中
                GameObject cardButton = Instantiate(card.GetPrefab(), cardPanel);
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
            UpdateDeckPanel(); // 每次抽牌后更新卡组显示

            yield return new WaitForSeconds(0.1f); // 每次抽牌后等待0.1秒
        }
    }

    public void UseCard(Card card)
    {
        hand.Remove(card);
        discardPile.Add(card);
        UpdateDiscardPileCountText(); // 更新弃牌堆数量显示

        // 找到并销毁已使用的卡牌按钮
        for (int i = cardButtons.Count - 1; i >= 0; i--)
        {
            CardButton cardButtonScript = cardButtons[i].GetComponent<CardButton>();
            if (cardButtonScript != null && cardButtonScript.GetCard() == card)
            {
                Destroy(cardButtons[i]);
                cardButtons.RemoveAt(i);
                break;
            }
        }
    }

    public void DiscardHand()
    {
        foreach (Card card in new List<Card>(hand))
        {
            UseCard(card);
        }
    }

    void ReshuffleDeck()
    {
        if (discardPile.Count > 0)
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
            UpdateDeckCountText(); // 更新牌库数量显示
            UpdateDiscardPileCountText(); // 更新弃牌堆数量显示
        }
    }

    public void UpdateDeckCountText()
    {
        if (deckCountText != null)
        {
            deckCountText.text = "Deck Count: " + deck.Count.ToString();
        }
    }

    public void UpdateDiscardPileCountText()
    {
        if (discardPileCountText != null)
        {
            discardPileCountText.text = "Discard Pile Count: " + discardPile.Count.ToString();
        }
    }

    public void UpdateDeckPanel()
    {
        // 清空当前显示的卡牌
        foreach (Transform child in deckPanel)
        {
            Destroy(child.gameObject);
        }

        // 显示牌库中的所有卡牌
        foreach (Card card in deck)
        {
            GameObject cardUI = new GameObject("Card");
            Image cardImage = cardUI.AddComponent<Image>();
            cardImage.sprite = card.GetSprite();

            RectTransform rectTransform = cardUI.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(40, 50); // 调整尺寸
            cardUI.transform.SetParent(deckPanel, false); // 保持相对布局

            Button cardButton = cardUI.AddComponent<Button>();
            cardButton.onClick.AddListener(() => OnCardClicked(card));
        }
    }

    void OnCardClicked(Card card)
    {
        Debug.Log("OnCardClicked called");

        if (player == null)
        {
            Debug.LogError("Player is not assigned.");
            return;
        }

        if (deletePopup == null)
        {
            Debug.LogError("Delete popup is not assigned.");
            return;
        }

        if (deletePopupMessage == null)
        {
            Debug.LogError("Delete popup message is not assigned.");
            return;
        }

        if (player.gold >= 20)
        {
            cardToDelete = card;
            deletePopupMessage.text = "Do you want to delete this card for 20 gold?";
            deletePopup.SetActive(true);
        }
        else
        {
            Debug.Log("Not enough gold to delete this card.");
        }
    }

    void ConfirmDeleteCard()
    {
        if (cardToDelete != null)
        {
            player.gold -= 20;
            player.UpdateGoldText();
            deck.Remove(cardToDelete);
            UpdateDeckCountText();
            UpdateDiscardPileCountText();
            UpdateDeckPanel();
            deletePopup.SetActive(false);
            cardToDelete = null;
        }
    }

    void CancelDeleteCard()
    {
        deletePopup.SetActive(false);
        cardToDelete = null;
    }

    public void UpdateHandDisplay()
    {
        // 清空当前显示的手牌
        foreach (Transform child in cardPanel)
        {
            Destroy(child.gameObject);
        }

        // 显示手牌中的所有卡牌
        foreach (Card card in hand)
        {
            GameObject cardButton = Instantiate(card.GetPrefab(), cardPanel);
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
    }

    public void HandleEndOfTurnEffects()
    {
        foreach (Card card in new List<Card>(hand))
        {
            if (card.hoardingValue > 0) // 检查囤积值
            {
                player.AddGold(card.hoardingValue);
                Debug.Log($"{card.Id} card's hoarding effect: +{card.hoardingValue} gold");
            }
    }
    }
    
}
