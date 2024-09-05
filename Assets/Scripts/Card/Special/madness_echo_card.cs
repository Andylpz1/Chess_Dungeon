using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class madness_echo_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void OnClick()
    {
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                player.currentCard = card;
                player.ExecuteCurrentCard();
                int cardsInHand = deckManager.hand.Count;
                deckManager.DiscardHand();
                deckManager.DrawCards(cardsInHand);
            }
        }
        else
        {
            Debug.LogError("Card is null in potion_card.OnClick");
        }
    }
}
