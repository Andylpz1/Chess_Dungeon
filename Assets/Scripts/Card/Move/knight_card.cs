using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class knight_card : CardButtonBase
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
                MoveHelper.ShowKnightMoveOptions(player, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in knight_card.OnClick");
        }
    }
}
