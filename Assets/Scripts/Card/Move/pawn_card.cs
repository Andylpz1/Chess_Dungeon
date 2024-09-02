using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class pawn_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        //Debug.Log("pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        //Debug.Log("pawn_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
                Debug.Log("Card deselected.");
            }
            else
            {
                MoveHelper.ShowPawnMoveOptions(player, card);
                Debug.Log("Showing pawn move options.");
            }
        }
        else
        {
            Debug.LogError("Card is null in pawn_card.OnClick");
        }
    }
}
