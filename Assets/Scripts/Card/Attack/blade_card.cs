using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class blade_card : CardButtonBase
{
    
    Vector2Int[] bladeDirections = 
    { 
        new Vector2Int(1, 1), 
        new Vector2Int(1, -1), 
        new Vector2Int(-1, 1), 
        new Vector2Int(-1, -1) 
    };

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
                player.ShowAttackOptions(bladeDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in blade_card.OnClick");
        }
    }
}
