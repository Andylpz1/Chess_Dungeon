using UnityEngine;

public class TwoBladeCard : Card
{
    public TwoBladeCard() : base(CardType.Attack, "A02A", 30, "A02")
    {
        isPartner = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/Blade_upgrade/twoblade_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/Blade_upgrade/twoblade_card");
    }
}