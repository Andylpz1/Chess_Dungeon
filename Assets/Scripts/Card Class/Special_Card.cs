using UnityEngine;

public class EnergyCore : Card
{
    public EnergyCore() : base(CardType.Special, "S02")
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

public class DarkEnergy : Card
{
    public DarkEnergy() : base(CardType.Special, "S03")
    {
        isQuick = true;
        isEnergy = true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/dark_energy_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/dark_energy_card");
    }
    public override string GetDescription()
    {
        return "快速，增加一点行动点，耗竭：抓一张牌";
    }
    public override void ExhaustEffect()
    {

        if (player != null)
        {
            player.deckManager.DrawCards(1);
        }
        else
        {
            Debug.LogError("Player is not assigned.");
        }
    }

}

public class MadnessEcho : Card
{
    public MadnessEcho() : base(CardType.Special,"S04")
    {
        isQuick= true;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/madness_echo_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/madness_echo_card");
    }
    public override string GetDescription()
    {
        return "快速，弃除所有手牌，抓等量的手牌";
    }
}

public class Vine : Card
{
    public Vine() : base(CardType.Special,"S05")
    {
        
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/vine_card");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/vine_card");
    }
    public override string GetDescription()
    {
        return "本回合中，每当你使用移动牌，尖棘都会对你相邻的一个敌人打1";
    }

}