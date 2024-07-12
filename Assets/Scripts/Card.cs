public enum CardType { Move, Attack }

[System.Serializable]
public class Card
{
    public CardType cardType;

    public Card(CardType type)
    {
        cardType = type;
    }

    public void Use(Player player)
    {
        switch (cardType)
        {
            case CardType.Move:
                // 移动效果处理逻辑
                player.ShowMoveOptions();
                break;
            case CardType.Attack:
                // 攻击效果处理逻辑
                // 添加攻击逻辑，例如显示攻击范围
                break;
        }
    }
}
