using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class pawn_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        //Debug.Log("pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void Start()
    {
        base.Start();

        if (card != null && card.IsUpgraded())
        {
            Transform glow = transform.Find("UpgradeEffect");
            if (glow != null)
                glow.gameObject.SetActive(true);
        }
    }
    
    protected override void OnClick()
    {
        //Debug.Log("pawn_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
                Debug.Log("Card deselected.");
            }
            else
            {
                MoveHelper.ShowPawnMoveOptions(player, card);
                Debug.Log("Showing pawn move options.");
            }
        }
        else
        {
            Debug.LogError("Card is null in pawn_card.OnClick");
        }
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

public class UpgradedPawnCard : Card
{
    public UpgradedPawnCard() : base(CardType.Move, "M01+", 5)
    {
        isQuick = true;
    }

    public override GameObject GetPrefab()
    {
        // 如果有单独的“升级版”预制体，可用类似 "pawn_card_upgraded"
        // 否则也可和普通 PawnCard 用同一个 prefab
        return Resources.Load<GameObject>("Prefabs/Card/Move/pawn_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/pawn_card_upgraded");
    }

    public override string GetDescription()
    {
        // 包含“快速”及“使用后抓一张牌”的提示
        return "P移动\n快速，使用后抓一张牌";
    }

    public override bool IsUpgraded()
    {
        // 标记为升级卡
        return true;
    }

    public override void OnCardExecuted()
    {
        base.OnCardExecuted();
        if (player != null && player.deckManager != null)
        {
            player.deckManager.DrawCards(1);
            Debug.Log("UpgradedPawnCard used: drew 1 card.");
        }
    }
}