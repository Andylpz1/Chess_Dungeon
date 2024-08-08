using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class spear_card : CardButtonBase
{

    Vector2Int[] spearDirections = 
    { 
        new Vector2Int(0, 1), 
        new Vector2Int(0, 2), 
        new Vector2Int(1, 0),
        new Vector2Int(2, 0),  
        new Vector2Int(-1, 0),
        new Vector2Int(-2, 0),  
        new Vector2Int(0, -1),
        new Vector2Int(0, -2)
    };

    public HintManager hintManager; // 引用HintManager

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
                player.ShowAttackOptions(spearDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in spear_card.OnClick");
        }
    }
}
