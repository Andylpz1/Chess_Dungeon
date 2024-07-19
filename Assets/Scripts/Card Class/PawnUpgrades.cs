using UnityEngine;

public class BanditCard : Card
{
    public BanditCard() : base(CardType.Move, "M01A", 20, "M01", true) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/Pawn_upgrade/bandit_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/Pawn_upgrade/bandit_card");
    }
}

public class SquireCard : Card
{
    public SquireCard() : base(CardType.Move, "M01B", 20, "M01", false, 10) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/Pawn_upgrade/squire_card");
    }
    
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/Pawn_upgrade/squire_card");
    }
}

public class LegionCard : Card
{
    public LegionCard() : base(CardType.Move, "M01C", 30, "M01") { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/Pawn_upgrade/legion_card");
    }
    
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/Pawn_upgrade/legion_card");
    }
}

