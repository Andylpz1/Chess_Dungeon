using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class DeckManager : MonoBehaviour
{
    public List<Card> deck; // 牌库
    public List<Card> hand; // 手牌
    public List<Card> discardPile; // 弃牌堆
    public int handSize = 5; // 手牌大小

    public Transform cardPanel; // 卡牌面板
    public Transform deckPanel; // 卡组面板，用于显示卡组中卡牌的图片
    public Transform discardPanel; // 弃牌堆面板，用于显示弃牌堆中的卡牌图片
    public Transform cardEditorPanel; //卡组编辑器面板

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

    public Button deckDisplayButton; // DeckDisplayButton 引用
    public Button discardDisplayButton; // DiscardDisplayButton 引用
    public List<Card> allCards = new List<Card>();

    void Start()
    {
        cardButtons = new List<GameObject>();
        discardPile = new List<Card>();
        InitializeDeck();
        InitializeCardEditor();
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

        //读取deckdisplayButton
        if (deckDisplayButton != null)
        {
            deckDisplayButton.onClick.AddListener(ToggleDeckDisplay);
        }
        else
        {
            Debug.LogError("DeckDisplayButton is not assigned in the Inspector.");
        }
        //读取discarddisplayButton
        if (discardDisplayButton != null)
        {
            discardDisplayButton.onClick.AddListener(ToggleDiscardDisplay);
        }
        else
        {
            Debug.LogError("DiscardDisplayButton is not assigned in the Inspector.");
        }

        // 确保 deckPanel 初始时显示和 discardPanel 初始时隐藏
        if (deckPanel != null)
        {
            deckPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("DeckPanel is not assigned in the Inspector.");
        }

        if (discardPanel != null)
        {
            discardPanel.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("DiscardPanel is not assigned in the Inspector.");
        }
        
    }

    void InitializeDeck()
    {
        // 初始化牌库
        deck = new List<Card>
        {
            //default deck
            new PawnCard(),
            new PawnCard(),
            new PawnCard(),
            new PawnCard(),
            new PawnCard(),
            new SwordCard(),
            new SwordCard(),
            new SwordCard()
            
            //step build
            //new Vine(),
            //new Vine(),
            //new PotionCard(),
            //new PotionCard(),
            //new EnergyCore(),
            //new EnergyCore(),
            //new Assassin(),
            //new Assassin(),
            //new Assassin(),
            //new TwoBladeCard(),
            //new TwoBladeCard(),
            //new TwoBladeCard(),
            //new TwoBladeCard()
            //new DarkEnergy(),
            //new FloatSword(),
            //new FloatSword(),
            //new FloatSword(),
            //new EnergyCore()
        };

        ShuffleDeck();

        player.SetDeck(deck);

    }

    void InitializeCardEditor()
    {
        allCards.Add(new KnightCard());
        allCards.Add(new BishopCard());
        allCards.Add(new RookCard());
        allCards.Add(new SwordCard());
        allCards.Add(new RitualSpear());
        allCards.Add(new EnergyCore());
        allCards.Add(new FloatSword());
        allCards.Add(new Vine());
        allCards.Add(new FlailCard());
        UpdateCardEditorPanel();
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
                UpdateDiscardPanel();
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
            UpdateDiscardPanel();
            if (player.actions == 0) 
            {
                
                player.DisableNonQuickCardButtons();
                
            }

            yield return new WaitForSeconds(0.1f); // 每次抽牌后等待0.1秒
        }
    }

    // 从特定位置抽卡
    public void DrawCardAt(int index)
    {
        if (index >= 0 && index < deck.Count)
        {
            Card drawnCard = deck[index];
            deck.RemoveAt(index);
            hand.Add(drawnCard);
            UpdateHandDisplay();
            UpdateDeckCountText(); // 每次抽牌后更新牌库剩余数量显示
            UpdateDeckPanel(); // 每次抽牌后更新卡组显示
            UpdateDiscardPanel();
        }
        else
        {
            Debug.LogError("Index out of range in DrawCardAt.");
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

    public void DiscardCard(int i)
    {
        // Ensure the index is valid before proceeding
        if (i < 0 || i >= hand.Count)
        {
            Debug.LogError("Invalid card index to discard: " + i);
            return;
        }

        // Get the card to be discarded
        Card card = hand[i];

        // Remove the card from hand and add it to the discard pile
        hand.RemoveAt(i);
        discardPile.Add(card);

        // Update the discard pile count UI
        UpdateDiscardPileCountText();

        // Trigger the discard effect of the card
        card.DiscardEffect();

        // Find the corresponding card button and destroy it
        // Check the card button by matching the card
        for (int j = cardButtons.Count - 1; j >= 0; j--)
        {
            CardButton cardButtonScript = cardButtons[j].GetComponent<CardButton>();
            if (cardButtonScript != null && cardButtonScript.GetCard() == card)
            {
                Destroy(cardButtons[j]); // Destroy the card's button
                cardButtons.RemoveAt(j); // Remove from the cardButtons list
                break;
            }
        }
    }



    public void DiscardHand()
    {
        for (int i = hand.Count - 1; i >= 0; i--)
        {
            DiscardCard(i); // Use DiscardCard method to discard each card
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

    public void RestartHand()
    {
        // Combine discard pile and hand into the deck
        deck.AddRange(discardPile);
        deck.AddRange(hand);

        // Clear the hand and discard pile
        hand.Clear();
        discardPile.Clear();

        UpdateHandDisplay();
        // Shuffle the deck
        ShuffleDeck();

        // Draw cards into hand
        //DrawCards(handSize);

        // Update the deck and discard pile UI (if you have any)
        UpdateDeckCountText();
        UpdateDiscardPileCountText();
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

    public void UpdateDiscardPanel()
    {
        // 清空当前显示的卡牌
        foreach (Transform child in discardPanel)
        {
            Destroy(child.gameObject);
        }

        // 显示弃牌堆中的所有卡牌
        foreach (Card card in discardPile)
        {
            GameObject cardUI = new GameObject("Card");
            Image cardImage = cardUI.AddComponent<Image>();
            cardImage.sprite = card.GetSprite();

            RectTransform rectTransform = cardUI.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(40, 50); // 调整尺寸
            cardUI.transform.SetParent(discardPanel, false); // 保持相对布局

            Button cardButton = cardUI.AddComponent<Button>();
            cardButton.onClick.AddListener(() => OnCardClicked(card));
        }
    }

    public void UpdateCardEditorPanel()
    {

    // 显示所有可用的卡牌
        foreach (Card card in allCards)
        {
            GameObject cardUI = new GameObject("Card");
            Image cardImage = cardUI.AddComponent<Image>();
            cardImage.sprite = card.GetSprite();

            RectTransform rectTransform = cardUI.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(40, 50); // 调整尺寸
            cardUI.transform.SetParent(cardEditorPanel, false); // 保持相对布局

            Button cardButton = cardUI.AddComponent<Button>();
            cardButton.onClick.AddListener(() => OnCardSelected(card));
        }
    }

    // 当卡牌被选中时调用的方法
    private void OnCardSelected(Card card)
    {
        deck.Add(card);
        UpdateDeckPanel(); // 更新牌组面板以显示新添加的卡牌
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
            player.SetDeck(deck);

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
        //确保没有步数的时候不会因为刷新手牌而给予额外行动机会
        if (player.actions == 0) 
        {
                
            player.DisableNonQuickCardButtons();
                
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
    
    public int GetAffinityCount(string Id)
    {
        return deck.Count(c => c.Id == Id) +
            hand.Count(c => c.Id == Id) +
            discardPile.Count(c => c.Id == Id);
    }

    public void ToggleDeckDisplay()
    {
        if (deckPanel != null)
        {
            bool isActive = deckPanel.gameObject.activeSelf;

            // 切换显示状态
            deckPanel.gameObject.SetActive(!isActive);

            // 如果deckPanel被激活，确保discardPanel被关闭
            if (deckPanel.gameObject.activeSelf && discardPanel != null)
            {
                discardPanel.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleDiscardDisplay()
    {
        if (discardPanel != null)
        {
            bool isActive = discardPanel.gameObject.activeSelf;

            // 切换显示状态
            discardPanel.gameObject.SetActive(!isActive);

            // 当展示弃牌堆时，更新显示内容并关闭deckPanel
            if (discardPanel.gameObject.activeSelf)
            {
                UpdateDiscardPanel();

                // 确保deckPanel被关闭
                if (deckPanel != null)
                {
                    deckPanel.gameObject.SetActive(false);
                }
            }
        }
    }

    public void Exhaust()
    {
        Debug.Log("Exhaust triggered. All cards with exhaust effects will be triggered.");
        
        List<Card> cardsToExhaust = new List<Card>(hand);

        // Iterate over the separate list and trigger the exhaust effects
        foreach (Card card in cardsToExhaust)
        {
            card.ExhaustEffect(); // Invoke the exhaust effect on each card
        }

        // 将 isCharged 设置为 false
        player.isCharged = false;
    }



}
