using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class dark_energy__card : CardButtonBase
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
