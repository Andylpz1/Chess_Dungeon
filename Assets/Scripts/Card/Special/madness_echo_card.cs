using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class madness_echo_card : CardButtonBase
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
                player.currentCard = card;
                player.ExecuteCurrentCard();
                deckManager.ExhaustCard(card);
                int cardsInHand = deckManager.hand.Count;
                deckManager.DiscardHand();
                deckManager.DrawCards(cardsInHand);
            }
        }
        else
        {
            Debug.LogError("Card is null in potion_card.OnClick");
        }
    }
}

public class MadnessEcho : Card
{
    public MadnessEcho() : base(CardType.Special,"S04")
    {
        isQuick= true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/madness_echo_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/madness_echo_card");
    }
    public override string GetDescription()
    {
        return "快速，消耗\n弃除所有手牌，抓等量的手牌";
    }
}
