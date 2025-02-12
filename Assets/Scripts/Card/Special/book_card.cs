using UnityEngine;

public class book_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("book_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("book_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                deckManager.DrawCards(2); // 抓取一张牌
                player.ExecuteCurrentCard();
                Debug.Log("Book card used: drew 1 card.");
            }
        }
        else
        {
            Debug.LogError("Card is null in book_card.OnClick");
        }
    }
}

public class Book : Card
{
    public Book() : base(CardType.Special, "S08")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/book_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/book_card");
    }
    public override string GetDescription()
    {
        return "抽两张牌";
    }
}