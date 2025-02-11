using UnityEngine;
using System.Collections.Generic;

public enum CardType { Move, Attack, Special}

[System.Serializable]
public class Card
{
    public Player player;
    public MonsterManager monsterManager;
    
    public CardType cardType;
    public string Id;
    public string cardName;
    public int cost; // 添加花费属性
    public bool isQuick; // 新增 quick 变量
    public bool isEnergy; // 新增 energy 变量
    public string upgradeFrom; // 升级来源
    public int hoardingValue; // 囤积值
    public bool isPartner;

    public Card(CardType type, string Id = "tbd", int cost = 10, string upgradeFrom = null, bool isQuick = false, int hoardingValue = 0, bool isPartner = false, bool isEnergy = false)
    {
        cardType = type;
        this.Id = Id;
        this.cost = cost;
        this.upgradeFrom = upgradeFrom;
        this.isQuick = isQuick;
        this.isEnergy = isEnergy;
        this.hoardingValue = hoardingValue;
        this.isPartner = isPartner;

        player = GameObject.FindObjectOfType<Player>();
        monsterManager = GameObject.FindObjectOfType<MonsterManager>();
    }

    public virtual GameObject GetPrefab()
    {
        return null;
    }

    public virtual Sprite GetSprite()
    {
        return null;
    }
    public virtual string GetDescription()
    {
        return null;
    }

    public virtual void ExhaustEffect()
    {
        // 这里可以加入每张卡牌独特的 Exhaust 效果
    }

    public virtual void DiscardEffect()
    {
        // 这里可以加入每张卡牌独特的 Discard 效果
    }

    public virtual void OnCardExecuted()
    {
        
    }

    public Card Clone()
    {
        // 简单的深拷贝，确保克隆出一个新实例
        return (Card)this.MemberwiseClone();
    }

    public string GetName()
    {
        return cardName;  // 假设每张卡牌有一个唯一名称
    }

}

public class PawnCard : Card
{
    public PawnCard() : base(CardType.Move, "M01", 5) { }

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
        return "P移动";
    }

}

public class KnightCard : Card
{
    public KnightCard() : base(CardType.Move, "M02", 20) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/knight_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/knight_card");
    }
    public override string GetDescription()
    {
        return "K移动";
    }

}

public class BishopCard : Card
{
    public BishopCard() : base(CardType.Move, "M03", 30) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/bishop_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/bishop_card");
    }
    public override string GetDescription()
    {
        return "B移动";
    }
}

public class RookCard : Card
{
    public RookCard() : base(CardType.Move, "M04", 50) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/rook_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/rook_card");
    }
    public override string GetDescription()
    {
        return "R移动";
    }
}

public class SwordCard : Card
{
    public SwordCard() : base(CardType.Attack, "A01", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/sword_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/sword_card");
    }
    public override string GetDescription()
    {
        return "上下左右攻击";
    }
    public override void OnCardExecuted()
    {
        //player.deckManager.DrawCards(1); // Sword card 特殊效果：抓两张牌
    }
}

public class BladeCard : Card
{
    public BladeCard() : base(CardType.Attack, "A02", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/blade_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/blade_card");
    }
    public override string GetDescription()
    {
        return "斜向攻击";
    }
}

public class SpearCard : Card
{
    public SpearCard() : base(CardType.Attack, "A03", 20) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/spear_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/spear_card");
    }
    public override string GetDescription()
    {
        return "上下左右两格攻击";
    }
}

public class BowCard : Card
{
    public BowCard() : base(CardType.Attack, "A04", 50) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/bow_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/bow_card");
    }
    public override string GetDescription()
    {
        return "任意位置攻击";
    }
}

//FlailCard在独立文件里

public class PotionCard : Card
{
    public PotionCard() : base(CardType.Special, "S01", 40)
    {
        isQuick = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/potion_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/potion_card");
    }
    public override string GetDescription()
    {
        return "快速，增加一点行动点";
    }
}

