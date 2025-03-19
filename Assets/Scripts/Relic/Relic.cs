using UnityEngine;

public abstract class Relic : ScriptableObject
{
    public string relicName;
    public string description;
    public Sprite icon;

    // 获得遗物时调用，应用遗物效果
    public abstract void OnAcquire(Player player);

    // 若游戏支持，每局开始重置遗物效果
    public virtual void OnGameStart(Player player) { }
}

[CreateAssetMenu(menuName = "Relics/Hand Size Relic")]
public class HandSizeRelic : Relic
{
    public override void OnAcquire(Player player)
    {
        player.deckManager.handSize = 7;
    
    }
    public override void OnGameStart(Player player)
    {
        player.deckManager.handSize = 7;
    }
}

[CreateAssetMenu(menuName = "Relics/Action Point Relic")]
public class ActionPointRelic : Relic
{
    public int actionPointIncrease = 1;

    public override void OnAcquire(Player player)
    {
        
    }
}

[CreateAssetMenu(menuName = "Relics/Health Relic")]
public class HealthRelic : Relic
{
    public int healthIncrease = 2;

    public override void OnAcquire(Player player)
    {
       
    }
}

[CreateAssetMenu(menuName = "Relics/Move Card Draw Relic")]
public class MoveCardDrawRelic : Relic
{
    private Player currentPlayer;
    private DeckManager deckManager;
    public override void OnAcquire(Player player)
    {
        currentPlayer = player;
        // 订阅玩家使用卡牌事件
        player.OnCardPlayed -= OnCardPlayed;
        player.OnCardPlayed += OnCardPlayed;
    }

    private void OnCardPlayed(Card playedCard)
    {
        if (playedCard.cardType == CardType.Move)
        {
            currentPlayer.deckManager.DrawCards(1);
            Debug.Log($"{relicName}效果触发：使用移动牌，额外抽一张牌。");
        }
    }

    // 可选：如果游戏是多局制，记得退订事件防止内存泄漏
    public override void OnGameStart(Player player)
    {
        currentPlayer = player;
        deckManager = player.deckManager;
        player.OnCardPlayed -= OnCardPlayed;
        player.OnCardPlayed += OnCardPlayed;
    }
    
}