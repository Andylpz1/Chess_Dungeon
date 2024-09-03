using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class float_sword : CardButtonBase
{
    Vector2Int[] swordDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private MonsterManager monsterManager;

    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        // Find the MonsterManager instance in the scene
        monsterManager = FindObjectOfType<MonsterManager>();
        if (monsterManager == null)
        {
            Debug.LogError("MonsterManager not found in the scene.");
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
                player.damage = 2;
                player.ShowAttackOptions(swordDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in float_sword.OnClick");
        }
    }
}
