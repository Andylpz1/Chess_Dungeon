using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ritual_spear : CardButtonBase
{

    Vector2Int[] spearDirections = 
    { 
        new Vector2Int(0, 1), 
        new Vector2Int(0, 2), 
        new Vector2Int(1, 0),
        new Vector2Int(2, 0),  
        new Vector2Int(-1, 0),
        new Vector2Int(-2, 0),  
        new Vector2Int(0, -1),
        new Vector2Int(0, -2)
    };

    public HintManager hintManager; // 引用HintManager

    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
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
                player.ShowAttackOptions(spearDirections, card);
            }
        }
        else
        {
        }
    }
}

public class RitualSpear : Card
{

    public RitualSpear() : base(CardType.Attack, "A08") 
    { 
        isMadness = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/ritual_spear_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/ritual_spear_card");
    }

    public override string GetDescription()
    {
        return "1点伤害 疯狂：对最近的敌人造成一点伤害";
    }

    public override void DiscardEffect()
    {

        if (monsterManager != null && player != null)
        {
            // Find and damage the nearest monster
            Monster nearestMonster = monsterManager.FindNearestMonster(player.position);
            if (nearestMonster != null)
            {
                nearestMonster.TakeDamage(1 + player.damageModifierThisTurn );
            }
            else
            {
            }
        }
        else
        {
        }
    }

}