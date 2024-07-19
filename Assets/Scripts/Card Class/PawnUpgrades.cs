using UnityEngine;

public class BanditCard : Card
{
    public BanditCard() : base(CardType.Move, "M01A", 5, true, "M01") { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/Pawn_upgrade/bandit_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/Pawn_upgrade/bandit_card");
    }
}
