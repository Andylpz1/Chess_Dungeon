using UnityEngine;
using System.Collections.Generic;

public class book_of_knight_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("book_of_knight_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("book_of_knight_card OnClick with card: " + (card != null ? card.ToString() : "null"));
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
                Debug.Log("Card deselected.");
            }
            else
            {
                player.currentCard = card;
                AttackKnightMoves();
                player.ExecuteCurrentCard();
                Debug.Log("Book of Knight card used: attacked knight move positions.");
            }
        }
        else
        {
            Debug.LogError("Card is null in book_of_knight_card.OnClick");
        }
    }

    private void AttackKnightMoves()
    {
        Vector2Int[] knightOffsets = new Vector2Int[]
        {
            new Vector2Int(-2, -1), new Vector2Int(-2, 1),
            new Vector2Int(2, -1), new Vector2Int(2, 1),
            new Vector2Int(-1, -2), new Vector2Int(-1, 2),
            new Vector2Int(1, -2), new Vector2Int(1, 2)
        };

        Vector2Int playerPosition = player.position;
        List<Vector2Int> attackPositions = new List<Vector2Int>();

        foreach (var offset in knightOffsets)
        {
            Vector2Int attackPosition = playerPosition + offset;
            if (player.IsValidPosition(attackPosition))
            {
                attackPositions.Add(attackPosition);
                Debug.Log("Attacked tile at: " + attackPosition);
            }
        }
        
        if (attackPositions.Count > 0)
        {
            player.MultipleAttack(attackPositions.ToArray());
        }
    }
}

public class BookOfKnight : Card
{
    public BookOfKnight() : base(CardType.Special, "S12")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/Book/book_of_knight");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/Book/book_of_knight");
    }
    public override string GetDescription()
    {
        return "攻击马位置的所有敌人";
    }
}
