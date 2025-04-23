using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class offering_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("offering_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("offering_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
                Debug.Log("Offering card deselected.");
            }
            else
            {
                player.currentCard = card;
                player.ExecuteCurrentCard();
                // 仪式逻辑
                Effects.KeywordEffects.StartBasicRitual();
                Effects.KeywordEffects.IncrementBasicRitual();
                // 抽一张牌
                deckManager.DrawCards(1, () => Debug.Log("Drew 1 card from Offering."));
                Debug.Log("Offering used: ritual progress and drew 1 card.");
            }
        }
        else
        {
            Debug.LogError("Card is null in offering_card.OnClick");
        }
    }
}

/// <summary>
/// 卡牌数据：祭品 (ID=T01)，临时、快速，战斗结束后移除。
/// </summary>
public class Offering : Card
{
    public Offering() : base(CardType.Special, "T01")
    {
        isQuick = true;
        isTemporary = true;
    }

    public override GameObject GetPrefab() => Resources.Load<GameObject>("Prefabs/Card/Special/offering_card");
    public override Sprite GetSprite() => Resources.Load<Sprite>("Sprites/Card/Special/offering_card");
    public override string GetDescription() => "临时牌 - 快速。打出时开启基础仪式, 抽1张牌。";
}
