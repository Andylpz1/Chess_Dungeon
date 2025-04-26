using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public class flame_sword_card : CardButtonBase
{
    Vector2Int[] flameSwordDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

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
                Debug.Log("FlameSwordCard deselected.");
            }
            else
            {
                int damage = card.GetDamageAmount();
                player.damage = damage;
                player.ShowAttackOptions(flameSwordDirections, card);
                Debug.Log("FlameSwordCard: Showing attack options with flame effect.");
            }
        }
        else
        {
            Debug.LogError("Card is null in flame_sword_card.OnClick");
        }
    }
}

public class FlameSword : Card
{
    // 构造函数中设置卡牌类型、编号和费用
    public FlameSword() : base(CardType.Attack, "A09", 1)
    {
    }
    
    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/flame_sword");
    }
    
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/flame_sword");
    }
    
    public override string GetDescription()
    {
        return "攻击目标并在其处铺设燃点地形";
    }
    
    public override void OnCardExecuted(Vector2Int gridPosition)
    {
        base.OnCardExecuted();
        
        // 攻击伤害的逻辑假设在基类或其他部分已经执行
        // 此处我们在攻击目标处铺设燃点
        
        Vector2Int targetPos = GetAttackTargetPosition();
        PlaceFirePointAt(gridPosition);
    }
    
    /// <summary>
    /// 获取攻击目标格子，取决于你的战斗系统
    /// 例如，这里假设 player 已经保存了本次攻击的目标位置
    /// </summary>
    private Vector2Int GetAttackTargetPosition()
    {
        return player.lastAttackSnapshot;
    }

    private void PlaceFirePointAt(Vector2Int gridPosition)
    {
        Debug.Log("Placing FirePoint at grid position: " + gridPosition);
        LocationManager locationManager = UnityEngine.Object.FindObjectOfType<LocationManager>();
        GameObject firePointPrefab = Resources.Load<GameObject>("Prefabs/Location/FirePoint");
        locationManager.CreateFirePoint(firePointPrefab, gridPosition);
    }
}
