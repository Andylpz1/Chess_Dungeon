using UnityEngine;

public class SickleCard : Card
{
    public SickleCard() : base(CardType.Attack, "A05", 10) { 
        isEnergy = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/sickle_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/sickle_card");
    }
    public override string GetDescription()
    {
        return "1点伤害，已充能：3点伤害";
    }
}

public class FloatSword : Card
{

    public FloatSword() : base(CardType.Attack) 
    { 
        isEnergy = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/float_sword_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/float_sword_card");
    }

    public override string GetDescription()
    {
        return "造成2点伤害，耗竭：对一个最近的敌人造成1点伤害";
    }

    public override void ExhaustEffect()
    {

        if (monsterManager != null && player != null)
        {
            // Find and damage the nearest monster
            Monster nearestMonster = monsterManager.FindNearestMonster(player.position);
            if (nearestMonster != null)
            {
                nearestMonster.TakeDamage(1);
                Debug.Log($"FloatSword card exhausted: Dealt 1 damage to {nearestMonster.name}");
            }
            else
            {
                Debug.Log("No monsters found nearby to damage.");
            }
        }
        else
        {
            Debug.LogError("MonsterManager or Player is not assigned.");
        }
    }

}