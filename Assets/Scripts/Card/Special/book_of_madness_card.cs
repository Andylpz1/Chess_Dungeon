using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class book_of_madness_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("book_of_madness_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("book_of_madness_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
                Debug.Log("Card deselected.");
            }
            else
            {
                player.currentCard = card;
                card.isQuick = true;
                DrawAndDiscard();

                Debug.Log("Book of Madness card used: drew 2 cards and discarded 1 randomly.");
            }
        }
        else
        {
            Debug.LogError("Card is null in book_of_madness_card.OnClick");
        }
    }

    private void DrawAndDiscard()
    {
        deckManager.DrawCards(2); // 先抽两张牌
        if (deckManager.hand.Count > 0) // 确保手牌不为空
        {
            int discardIndex = Random.Range(0, deckManager.hand.Count); // 选择随机索引
            deckManager.DiscardCard(discardIndex); // 丢弃索引对应的牌
            Debug.Log("Randomly discarded card at index: " + discardIndex);
        }
    }

    


}

public class BookOfMadness : Card
{
    public BookOfMadness() : base(CardType.Special, "S10")
    {
        isQuick = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/book_of_madness_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/book_of_madness_card");
    }
    public override string GetDescription()
    {
        return "快速，抽两张牌，随机弃一张牌";
    }
}