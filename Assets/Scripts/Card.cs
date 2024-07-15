public enum CardType { Move, Attack }
public enum MoveType { Pawn, Knight }
[System.Serializable]
public class Card
{
    public CardType cardType;
    public MoveType moveType;

    public Card(CardType type, MoveType moveType = MoveType.Pawn)
    {
        cardType = type;
        this.moveType = moveType;
    }

}

public class PawnCard : Card
{
    public PawnCard() : base(CardType.Move, MoveType.Pawn) { }
}

public class KnightCard : Card
{
    public KnightCard() : base(CardType.Move, MoveType.Knight) { }
}

public class AttackCard : Card
{
    public AttackCard() : base(CardType.Attack) { }
}