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
                    MoveHelper.ShowRookMoveOptions(player, card);
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

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("P移动，富裕(100)：R移动", transform.position);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
}
