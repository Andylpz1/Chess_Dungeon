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