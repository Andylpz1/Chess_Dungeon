using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


public enum CardUpgrade
{
    Quick,      // 卡牌变为快速
    Draw1,      // 使用后抓1张牌
    Draw2,      // 使用后抓2张牌
    GainArmor   // 使用后额外获得1点护甲
}

public class pawn_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        //Debug.Log("pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void Start()
    {
        base.Start();

        if (card != null && card.IsUpgraded())
        {
            Transform glow = transform.Find("UpgradeEffect");
            if (glow != null)
                glow.gameObject.SetActive(true);
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
                Debug.Log("Card deselected.");
            }
            else
            {
                MoveHelper.ShowPawnMoveOptions(player, card);
                Debug.Log("Showing pawn move options.");
            }
        }
        else
        {
            Debug.LogError("Card is null in pawn_card.OnClick");
        }
    }
}

public class PawnCard : Card
{
    public List<CardUpgrade> upgrades = new List<CardUpgrade>();
    public override List<CardUpgrade> UpgradeOptions { get; protected set; } = new List<CardUpgrade>();
    public PawnCard() : base(CardType.Move, "M01", 5) 
    {
        // 为移动牌配置个性化的升级选项
        UpgradeOptions = new List<CardUpgrade>
        {
            CardUpgrade.Quick,
            CardUpgrade.Draw1,
            CardUpgrade.Draw2,
            CardUpgrade.GainArmor
        };
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/pawn_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/pawn_card");
    }
     public override string GetDescription()
    {
        string desc = "P移动";
        // 如果有升级，则显示所有升级效果
        if (upgrades.Count > 0)
        {
            desc += "\n升级效果：";
            foreach (CardUpgrade upgrade in upgrades)
            {
                switch (upgrade)
                {
                    case CardUpgrade.Quick:
                        desc += "\n快速";
                        break;
                    case CardUpgrade.Draw1:
                        desc += "\n使用后抓1张牌";
                        break;
                    case CardUpgrade.Draw2:
                        desc += "\n使用后抓2张牌";
                        break;
                    case CardUpgrade.GainArmor:
                        desc += "\n额外获得1点护甲";
                        break;
                }
            }
        }
        return desc;
    }
    /// 外部调用该方法给这张 Pawn 卡添加一个升级（升级效果可以任意组合和重复）
    public override void AddUpgrade(CardUpgrade upgrade)
    {
        upgrades.Add(upgrade);
        // 如果选择了快速升级，则设为快速
        if (upgrade == CardUpgrade.Quick)
        {
            isQuick = true;
        }
        string suffix = "+" + upgrade;           // 例如 “+Draw1”
        if (!Id.Contains(suffix))
            Id += suffix;                   // M01 → M01+Draw1(+Quick …)

        Debug.Log($"AddUpgrade → 新 Id = {Id}");
    }

    public override bool IsUpgraded()
    {
        return upgrades.Count > 0;
    }

    /// 当卡牌真正使用后，根据升级列表累计执行各项效果。
    public override void OnCardExecuted()
    {
        base.OnCardExecuted();

        // 统计抽牌效果
        int totalDraw = 0;
        foreach (CardUpgrade upgrade in upgrades)
        {
            if (upgrade == CardUpgrade.Draw1)
                totalDraw += 1;
            else if (upgrade == CardUpgrade.Draw2)
                totalDraw += 2;
        }
        if (player != null && player.deckManager != null && totalDraw > 0)
        {
            player.deckManager.DrawCards(totalDraw);
        }


        // 统计额外获得护甲的效果
        int armorGained = 0;
        foreach (CardUpgrade upgrade in upgrades)
        {
            if (upgrade == CardUpgrade.GainArmor)
                armorGained++;
        }
        if (armorGained > 0 && player != null)
        {
            player.AddArmor(armorGained);
        }
    }
}

public class UpgradedPawnCard : Card
{
    public UpgradedPawnCard() : base(CardType.Move, "M01+", 5)
    {
        isQuick = true;
    }

    public override GameObject GetPrefab()
    {
        // 如果有单独的“升级版”预制体，可用类似 "pawn_card_upgraded"
        // 否则也可和普通 PawnCard 用同一个 prefab
        return Resources.Load<GameObject>("Prefabs/Card/Move/pawn_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/pawn_card_upgraded");
    }

    public override string GetDescription()
    {
        // 包含“快速”及“使用后抓一张牌”的提示
        return "P移动\n快速，使用后抓一张牌";
    }

    public override bool IsUpgraded()
    {
        // 标记为升级卡
        return true;
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted();
        if (player != null && player.deckManager != null)
        {
            player.deckManager.DrawCards(1);
            Debug.Log("UpgradedPawnCard used: drew 1 card.");
        }
    }
}