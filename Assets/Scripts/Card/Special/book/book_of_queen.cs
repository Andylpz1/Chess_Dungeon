using UnityEngine;
using System.Collections.Generic;

public class book_of_queen_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("book_of_queen_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("book_of_queen_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                AttackSurroundingTiles();
                player.ExecuteCurrentCard();
                Debug.Log("Book of Queen card used: attacked all surrounding tiles.");
            }
        }
        else
        {
            Debug.LogError("Card is null in book_of_queen_card.OnClick");
        }
    }

    private void AttackSurroundingTiles()
    {
        List<Vector2Int> attackPositions = new List<Vector2Int>();
        Vector2Int playerPosition = player.position;

        // 身边8格方向（上下左右+四个斜角）
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right,
            new Vector2Int(-1, -1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(1, 1)
        };

        foreach (var direction in directions)
        {
            Vector2Int attackPosition = playerPosition + direction;
            if (player.IsValidPosition(attackPosition))
            {
                attackPositions.Add(attackPosition);
            }
        }

        if (attackPositions.Count > 0)
        {
            player.MultipleAttack(attackPositions.ToArray());
        }
    }
}

public class BookOfQueen : Card
{
    public BookOfQueen() : base(CardType.Special, "S15")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/Book/book_of_queen");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/Book/book_of_queen");
    }
    public override string GetDescription()
    {
        return "攻击身边8格的所有敌人";
    }
}
