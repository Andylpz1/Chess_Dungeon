using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Effects;
public class sword_card : CardButtonBase
{

    Vector2Int[] swordDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };


    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
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
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                int damage = card.GetDamageAmount(); 
                player.damage = damage;
                player.ShowAttackOptions(swordDirections,card);
                
            }
        }
        else
        {
            Debug.LogError("Card is null in attack_card.OnClick");
        }
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

    public override string GetDescription()
    {
        return "上下左右攻击，并对目标格内的敌人进行击退";
    }

    public override void OnCardExecuted()
    {
        KeywordEffects.AttackWithKnockback(player);
        
    }

}


public class UpgradedSwordCard : Card
{
    public UpgradedSwordCard() : base(CardType.Attack, "A01+", 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/sword_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/sword_card_upgraded");
    }

    public override string GetDescription()
    {
        return "上下左右攻击,造成2点伤害";
    }

    public override void OnCardExecuted()
    {
        // 升级剑卡可能也有额外效果（比如抽卡）
    }

    public override int GetDamageAmount()
    {
        return 2; // 与普通剑卡区分
    }

    public override bool IsUpgraded()
    {
        return true;
    }
}
