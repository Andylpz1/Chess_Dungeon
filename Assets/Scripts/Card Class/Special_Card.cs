using UnityEngine;

public class EnergyCore : Card
{
    public EnergyCore() : base(CardType.Special)
    {
        isEnergy= true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/energy_core");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/energy_core");
    }
    public override string GetDescription()
    {
        return "抓两张牌，充能；已充能：抓四张牌";
    }
}
