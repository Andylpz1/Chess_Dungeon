using UnityEngine;
using System.Collections;

// 卡牌点击按钮的脚本
public class warfire_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
    }

    protected override void OnClick()
    {
        if (card != null)
        {
            // 如果已经选中同一张牌，则取消选中
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                // 选中这张牌，并应用“战火”效果
                player.currentCard = card;
                player.ExecuteCurrentCard();
                ApplyWarFireEffect();
            }
        }
        else
        {
            Debug.LogError("Card is null in war_fire_card.OnClick");
        }
    }

    private void ApplyWarFireEffect()
    {
        if (player != null)
        {
            player.damageModifierThisTurn += 1; 
        }
        else
        {
            Debug.LogWarning("ApplyWarFireEffect: player is null, cannot apply fury effect.");
        }
    }
}

// 与之对应的卡牌类本体
public class WarFire : Card
{
    public WarFire() : base(CardType.Special, "S18") // 编号可根据需要自定义
    {
        // 若你想让它也是“快速”牌，就改为 isQuick = true;
        // 此示例留作普通特殊牌
        isQuick = false;
    }

    public override GameObject GetPrefab()
    {
        // 预制体的路径示例，请根据项目实际资源进行设置
        return Resources.Load<GameObject>("Prefabs/Card/Special/warfire_card");
    }

    public override Sprite GetSprite()
    {
        // 卡面图片路径示例
        return Resources.Load<Sprite>("Sprites/Card/Special/warfire_card");
    }

    public override string GetDescription()
    {
        return "本回合内，你获得“愤怒”（造成的伤害+1）";
    }
}
