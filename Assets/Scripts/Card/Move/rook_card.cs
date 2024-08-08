using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class rook_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
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
                MoveHelper.ShowRookMoveOptions(player, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in rook_card.OnClick");
        }
    }
}
