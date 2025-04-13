using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        // 判断目标攻击位置是否有怪物
        Monster targetMonster = GetMonsterAtPosition(player.targetAttackPosition);
        if (targetMonster != null)
        {
            // 根据玩家与怪物之间的相对位置计算方向（使用归一化向量）
            Vector2 direction = (targetMonster.transform.position - player.transform.position).normalized;
            // 将任意方向向量转换为最近的上下左右方向
            Vector2 cardinalDirection = RoundToCardinal(direction);
            // 应用击退效果：试图将怪物击退 1 格，如果目标位置不可到达则造成 1 点伤害
            ApplyKnockback(targetMonster, cardinalDirection);
        }
    }

    private Monster GetMonsterAtPosition(Vector2Int pos)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(pos))
            {
                return monster;
            }
        }
        return null;
    }

    private Vector2 RoundToCardinal(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            return new Vector2(Mathf.Sign(direction.x), 0);
        }
        else
        {
            return new Vector2(0, Mathf.Sign(direction.y));
        }
    }

    private void ApplyKnockback(Monster target, Vector2 direction)
    {
        Vector2Int currentPos = target.position;
        Vector2Int knockbackDir = new Vector2Int((int)direction.x, (int)direction.y);
        Vector2Int desiredPos = currentPos + knockbackDir;
        
        // 判断 desiredPos 是否有效：必须在棋盘内且不被阻挡
        if (IsPositionValid(desiredPos))
        {
            target.position = desiredPos;
            target.UpdatePosition();
        }
        else
        {
            target.TakeDamage(1);
        }
    }

    /// <summary>
    /// 判断目标位置是否有效
    /// 使用 player 中已有的方法进行判断：
    /// 1. 必须在棋盘范围内（player.IsValidPosition）
    /// 2. 该位置未被怪物或不可进入的地形阻挡（!player.IsBlockedBySomething）
    /// </summary>
    private bool IsPositionValid(Vector2Int pos)
    {
        return player.IsValidPosition(pos) && !player.IsBlockedBySomething(pos);
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
