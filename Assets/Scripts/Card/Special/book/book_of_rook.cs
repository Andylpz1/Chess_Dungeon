using UnityEngine;
using System.Collections.Generic;

public class book_of_rook_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("book_of_rook_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("book_of_rook_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                AttackRookMoves();
                player.ExecuteCurrentCard();
                Debug.Log("Book of Rook card used: attacked straight line positions.");
            }
        }
        else
        {
            Debug.LogError("Card is null in book_of_rook_card.OnClick");
        }
    }

    private void AttackRookMoves()
    {
        List<Vector2Int> attackPositions = new List<Vector2Int>();
        Vector2Int playerPosition = player.position;

        // 直线方向（上下左右）
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        foreach (var direction in directions)
        {
            for (int i = 1; i < player.boardSize; i++)
            {
                Vector2Int attackPosition = playerPosition + direction * i;
                if (player.IsValidPosition(attackPosition))
                {
                    attackPositions.Add(attackPosition);
                }
                else
                {
                    break; // 遇到边界停止攻击延展
                }
            }
        }

        if (attackPositions.Count > 0)
        {
            player.MultipleAttack(attackPositions.ToArray());
        }
    }
}

public class BookOfRook : Card
{
    public BookOfRook() : base(CardType.Special, "S14")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/Book/book_of_rook");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/Book/book_of_rook");
    }
    public override string GetDescription()
    {
        return "攻击直线方向的所有敌人";
    }
}
