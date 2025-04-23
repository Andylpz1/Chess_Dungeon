using UnityEngine;
using Effects;

/// <summary>
/// Handles the UI and execution logic for the Ritual Dagger card (A11),
/// which lets the player choose one diagonal to strike and then grants an Offering.
/// </summary>
public class ritual_dagger_card : CardButtonBase
{
    // Four diagonal directions to choose from
    private readonly Vector2Int[] daggerDirections = {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log($"ritual_dagger_card Initialize with card: {card?.Id}");
    }

    protected override void Start()
    {
        base.Start();
        if (card != null && card.IsUpgraded())
        {
            var glow = transform.Find("UpgradeEffect");
            if (glow != null)
                glow.gameObject.SetActive(true);
        }
    }

    protected override void OnClick()
    {
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                int damage = card.GetDamageAmount(); 
                player.damage = damage;
                player.ShowAttackOptions(daggerDirections,card);
                
            }
        }
    }
}

/// <summary>
/// Card data for Ritual Dagger (A11): choose one diagonal to deal 1 damage, then add an Offering to hand.
/// </summary>
public class RitualDagger : Card
{
    public RitualDagger() : base(CardType.Attack, "A11", /*damage=*/1) { }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Attack/ritual_dagger_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Attack/ritual_dagger_card");
    }

    public override string GetDescription()
    {
        return "选择一个对角格，对其内的敌人造成1点伤害；打出后将一张祭品添加到手牌。";
    }

    /// <summary>
    /// Executes the selected diagonal attack and then adds an Offering to hand.
    /// </summary>
    public override void OnCardExecuted()
    {
        // Get the chosen target position
        Vector2Int targetPos = Player.Instance.targetAttackPosition;

        // Deal damage if an enemy is present
        var monster = KeywordEffects.GetMonsterAtPosition(targetPos);
        if (monster != null)
            // Add one Offering card to hand
            Player.Instance.deckManager.AddCardToHand(new Offering());
    }
}
