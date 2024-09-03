using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class sickle: CardButtonBase
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
                if (player.isCharged) {
                    player.damage = 3;
                }
                player.ShowAttackOptions(swordDirections,card);
            }
        }
        else
        {
            Debug.LogError("Card is null in attack_card.OnClick");
        }
    }

}

