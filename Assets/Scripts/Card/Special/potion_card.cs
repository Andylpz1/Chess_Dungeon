using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class potion_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("potion_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("potion_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                turnManager.AddAction();
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
