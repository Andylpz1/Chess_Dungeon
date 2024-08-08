using UnityEngine;
using System.Collections.Generic;

public enum CardType { Move, Attack, Special}

[System.Serializable]
public class Card
{
    public CardType cardType;
    public string Id;
    public int cost; // 添加花费属性
    public bool isQuick; // 新增 quick 变量
    public string upgradeFrom; // 升级来源
    public int hoardingValue; // 囤积值
    public bool isPartner;

    public Card(CardType type, string Id = "M01", int cost = 10, string upgradeFrom = null, bool isQuick = false, int hoardingValue = 0, bool isPartner = false)
    {
        cardType = type;
        this.Id = Id;
        this.cost = cost;
        this.upgradeFrom = upgradeFrom;
        this.isQuick = isQuick;
        this.hoardingValue = hoardingValue;
        this.isPartner = isPartner;
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
}

