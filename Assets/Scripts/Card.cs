using UnityEngine;

public enum CardType { Move, Attack }

[System.Serializable]
public class Card
{
    public CardType cardType;
    public string Id;

    public Card(CardType type, string Id = "M01")
    {
        cardType = type;
        this.Id = Id;
    }

    public virtual GameObject GetPrefab()
    {
        return null;
    }
}

public class PawnCard : Card
{
    public PawnCard() : base(CardType.Move, "M01") { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/pawn_card");;
    }
}

public class KnightCard : Card
{
    public KnightCard() : base(CardType.Move, "A01") { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/knight_card");;
    }
}

public class AttackCard : Card
{
    public AttackCard() : base(CardType.Attack, "A02") { }


    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/attack_card");;
    }
}
