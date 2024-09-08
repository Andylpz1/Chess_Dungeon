using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class assassin_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        //Debug.Log("pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
        if (player == null)
        {
            player = GameObject.FindObjectOfType<Player>();
            if (player == null)
            {
                Debug.LogError("Player not found in the scene.");
                return; // Exit if player is not found
            }
        }
        player.OnCardPlayed += CheckComboStatus;
    }

    private void CheckComboStatus()
    {
        if (player.cardsUsedThisTurn >= 3)
        {
            card.isQuick = true; // Make the assassin card quick after 3 cards are used
            Debug.Log("Assassin card is now quick due to combo.");
        }
    }

    protected override void OnClick()
    {
        //Debug.Log("pawn_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                MoveHelper.ShowPawnMoveOptions(player, card);
            }
        }
        else
        {
        }
    }
}
