using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class gentleman_card : pawn_card
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
                if (player.gold >= 100)
                {
                    MoveHelper.ShowKnightMoveOptions(player, card);
                }
                else 
                {
                    MoveHelper.ShowPawnMoveOptions(player, card);
                }
            }
        }
        else
        {
            Debug.LogError("Card is null in gentleman_card.OnClick");
        }
    }


}
