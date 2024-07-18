using UnityEngine;

public enum CardType { Move, Attack, Special}

[System.Serializable]
public class Card
{
    public CardType cardType;
    public string Id;
    public int cost; // 添加花费属性

    public Card(CardType type, string Id = "M01", int cost = 10)
    {
        cardType = type;
        this.Id = Id;
        this.cost = cost;
    }

    public virtual GameObject GetPrefab()
    {
        return null;
    }

    public virtual Sprite GetSprite()
    {
        return null;
    }
}

public class PawnCard : Card
{
    public PawnCard() : base(CardType.Move, "M01", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/pawn_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/pawn_card");
    }
}

public class KnightCard : Card
{
    public KnightCard() : base(CardType.Move, "M02", 40) { }

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
    public BishopCard() : base(CardType.Move, "M03", 60) { }

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
    public RookCard() : base(CardType.Move, "M04", 60) { }

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
    public SwordCard() : base(CardType.Attack, "A01", 20) { }

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
    public BladeCard() : base(CardType.Attack, "A02", 20) { }

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
    public SpearCard() : base(CardType.Attack, "A03", 40) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/spear_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/spear_card");
    }
}

