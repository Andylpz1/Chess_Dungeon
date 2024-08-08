using UnityEngine;

public class DaggerCard : Card
{
    public DaggerCard() : base(CardType.Attack, "A01A", 30, "A01")
    {
        isQuick = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/Sword_upgrade/dagger_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/Sword_upgrade/dagger_card");
    }
    public override string GetDescription()
    {
        return "快速，上下左右攻击";
    }
}