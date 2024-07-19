using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class legion_card : pawn_card
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
                if (deckManager.GetAffinityCount("M01C") >= 3)
                {
                    MoveHelper.ShowRookMoveOptions(player, card);
                }
                else 
                {
                    player.ShowMoveOptions(pawnDirections, card);
                }
            }
        }
        else
        {
            Debug.LogError("Card is null in legion_card.OnClick");
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("P移动，共鸣(3)：R移动", transform.position);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
