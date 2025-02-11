using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class coffin_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("coffin_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("coffin_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                DrawMadnessCards(2);
                player.ExecuteCurrentCard();
                Debug.Log("Coffin card used: drew 2 madness cards and executed.");
            }
        }
        else
        {
            Debug.LogError("Card is null in coffin_card.OnClick");
        }
    }

    private void DrawMadnessCards(int count)
    {
        List<Card> madnessCards = deckManager.deck.Where(c => c.isMadness).ToList();

        if (madnessCards.Count == 0)
        {
            Debug.Log("No madness cards available to draw.");
            return;
        }

        for (int i = 0; i < count && madnessCards.Count > 0; i++)
        {
            Card drawnCard = madnessCards[0];  // 取第一张 madness 牌
            madnessCards.RemoveAt(0);
            deckManager.DrawSpecificCard(drawnCard);
        }
    }

}
