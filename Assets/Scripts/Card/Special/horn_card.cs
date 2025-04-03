using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class horn_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("horn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("horn_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            // 如果玩家当前选中的是同一张牌，则取消选择
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
                Debug.Log("Card deselected.");
            }
            else
            {
                // 选中本牌并执行号角效果：抽4张牌，然后弃掉所有非攻击牌
                player.currentCard = card;
                player.ExecuteCurrentCard();
                HornEffect();

                Debug.Log("Horn card used: drew 4 cards and discarded all non-attack cards from hand.");
            }
        }
        else
        {
            Debug.LogError("Card is null in horn_card.OnClick");
        }
    }

    private void HornEffect()
    {
        deckManager.DrawCards(4, () =>
        {
            // 这里才是真正抽完牌之后的逻辑
            for (int i = deckManager.hand.Count - 1; i >= 0; i--)
            {
                Card handCard = deckManager.hand[i];
                if (handCard.cardType != CardType.Attack)
                {
                    deckManager.DiscardCard(i);
                }
            }
            Debug.Log("Drew 4 cards, discarded all non-attack cards.");
        });
    }

}

public class Horn : Card
{
    public Horn() : base(CardType.Special, "S17")
    {
        // 根据需要决定是否让号角为“快速”，此处示例设为false
        isQuick = false;
    }

    // 预制体与贴图路径示例，请根据项目实际资源配置调整
    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/horn_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/horn_card");
    }

    public override string GetDescription()
    {
        return "抽四张牌，然后弃掉手牌中的所有非攻击牌";
    }
}
