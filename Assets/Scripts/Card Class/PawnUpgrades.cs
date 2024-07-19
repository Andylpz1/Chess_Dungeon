using UnityEngine;

public class BanditCard : Card
{
    public BanditCard() : base(CardType.Move, "M01A", 20, "M01")
    {
        isQuick = true;
    }

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
    public SquireCard() : base(CardType.Move, "M01B", 20, "M01")
    {
        hoardingValue = 10;
    }

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
    public LegionCard() : base(CardType.Move, "M01C", 40, "M01")
    {

    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/Pawn_upgrade/legion_card");
    }
    
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/Pawn_upgrade/legion_card");
    }
}

public class MercenaryCard : Card
{
    public MercenaryCard() : base(CardType.Move, "M01D", 40, "M01")
    {
        isQuick = true;
        isPartner = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/Pawn_upgrade/mercenary_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/Pawn_upgrade/mercenary_card");
    }
}

public class GentlemanCard : Card
{
    public GentlemanCard() : base(CardType.Move, "M01E", 10, "M01")
    {

    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Move/Pawn_upgrade/gentleman_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Move/Pawn_upgrade/gentleman_card");
    }
}

