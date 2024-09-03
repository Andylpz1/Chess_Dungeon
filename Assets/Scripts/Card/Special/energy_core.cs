using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class energy_core : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void OnClick()
    {
        Debug.Log("potion_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                player.currentCard = card;
                if (!player.isCharged) {
                    player.deckManager.DrawCards(2);
                    player.Charge();
                }
                else {
                    player.deckManager.DrawCards(4);
                }
                
                player.ExecuteCurrentCard();
                Debug.Log("Potion card used: action added and current card executed.");
            }
        }
        else
        {
            Debug.LogError("Card is null in potion_card.OnClick");
        }
    }
}
