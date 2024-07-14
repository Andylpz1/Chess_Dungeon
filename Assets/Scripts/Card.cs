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

    public void ShowOptions(Player player)
    {
        switch (cardType)
        {
            case CardType.Move:
                switch (moveType)
                {
                    case MoveType.Pawn:
                        player.ShowMoveOptions(this);
                        break;
                    case MoveType.Knight:
                        player.ShowKnightMoveOptions(this);
                        break;
                }
                break;
            case CardType.Attack:
                player.ShowAttackOptions(this);
                break;
        }
    }

    public void Use(Player player)
    {
        // Use 方法不再显示高亮位置，只执行动作
        switch (cardType)
        {
            case CardType.Move:
                switch (moveType)
                {
                    case MoveType.Pawn:
                        player.Move(player.position); // pawn move
                        break;
                    case MoveType.Knight:
                        player.Move(player.position); // knight move
                        break;
                }
                break;
            case CardType.Attack:
                player.Attack(player.position); // attack
                break;
        }
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