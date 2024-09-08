using UnityEngine;

public class Assassin : Card
{
    public Assassin() : base(CardType.Move)
    {
        isQuick = false;
        isPartner = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/assassin_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/assassin_card");
    }
    public override string GetDescription()
    {
        return "拍档，p移动； 连击3：快速";
    }
}