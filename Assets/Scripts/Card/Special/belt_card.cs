using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class belt_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("belt_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("belt_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                DrawAttackCards(2);
                player.ExecuteCurrentCard();
                Debug.Log("Belt card used: drew 2 attack cards.");
            }
        }
        else
        {
            Debug.LogError("Card is null in belt_card.OnClick");
        }
    }

    private void DrawAttackCards(int count)
    {
        List<Card> attackCards = deckManager.deck.Where(c => c.cardType == CardType.Attack).Take(count).ToList();
        foreach (var attackCard in attackCards)
        {
            deckManager.DrawSpecificCard(attackCard);
            Debug.Log("Drew attack card: " + attackCard.cardName); // 使用 cardName 而不是 name
        }
    }

}

public class Belt : Card
{
    public Belt() : base(CardType.Special, "S09")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/belt_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/belt_card");
    }
    public override string GetDescription()
    {
        return "抽两张攻击牌";
    }
}