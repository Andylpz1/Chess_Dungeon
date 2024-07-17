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
        return Resources.Load<GameObject>("Prefabs/Card/pawn_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/pawn_card");
    }
}

public class KnightCard : Card
{
    public KnightCard() : base(CardType.Move, "M02", 40) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/knight_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/knight_card");
    }
}

public class BishopCard : Card
{
    public BishopCard() : base(CardType.Move, "M03", 60) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/bishop_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/bishop_card");
    }
}

public class RookCard : Card
{
    public RookCard() : base(CardType.Move, "M04", 60) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/rook_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/rook_card");
    }
}

public class SwordCard : Card
{
    public SwordCard() : base(CardType.Attack, "A01", 20) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/sword_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/sword_card");
    }
}
