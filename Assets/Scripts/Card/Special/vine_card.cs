using UnityEngine;

public class vine_card : CardButtonBase
{
    private bool isVineActive = false; // Tracks if the Vine effect is active for this turn

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
                Debug.Log("Vine card deselected. Effect deactivated.");
            }
            else
            {
                player.currentCard = card;
                player.vineEffectActive = true;
                Debug.Log("Vine card activated. Vine effect is active.");

                player.ExecuteCurrentCard(); // Perform default behavior for the card
            }
        }
        else
        {
            Debug.LogError("Card is null in vine_card.OnClick");
        }
    }

}
