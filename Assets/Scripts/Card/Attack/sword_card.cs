using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class sword_card : CardButtonBase
{

    Vector2Int[] swordDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };


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
                player.ShowAttackOptions(swordDirections,card);
                
            }
        }
        else
        {
            Debug.LogError("Card is null in attack_card.OnClick");
        }
    }


}

