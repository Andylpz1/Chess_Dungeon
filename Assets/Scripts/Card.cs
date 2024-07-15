using UnityEngine;

public enum CardType { Move, Attack }

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

public class AttackCard : Card
{
    public AttackCard() : base(CardType.Attack, "A01", 20) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/attack_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/attack_card");
    }
}
