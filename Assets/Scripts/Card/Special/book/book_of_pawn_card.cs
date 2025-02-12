using UnityEngine;
using System.Collections.Generic;

public class book_of_pawn_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("book_of_pawn_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("book_of_pawn_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                AttackDiagonalTiles();
                player.ExecuteCurrentCard();
                Debug.Log("Book of Pawn card used: attacked four diagonal tiles.");
            }
        }
        else
        {
            Debug.LogError("Card is null in book_of_pawn_card.OnClick");
        }
    }

    private void AttackDiagonalTiles()
    {
        Vector2Int[] diagonalOffsets = new Vector2Int[]
        {
            new Vector2Int(-1, -1), // 左下
            new Vector2Int(-1, 1),  // 左上
            new Vector2Int(1, -1),  // 右下
            new Vector2Int(1, 1)    // 右上
        };

        Vector2Int playerPosition = player.position;
        List<Vector2Int> attackPositions = new List<Vector2Int>();

        foreach (var offset in diagonalOffsets)
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

public class BookOfPawn : Card
{
    public BookOfPawn() : base(CardType.Special, "S11")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/Book/book_of_pawn");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/Book/book_of_pawn");
    }
    public override string GetDescription()
    {
        return "攻击四个斜角位置的敌人";
    }
}