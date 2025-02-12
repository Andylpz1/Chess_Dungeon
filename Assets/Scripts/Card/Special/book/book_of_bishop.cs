using UnityEngine;
using System.Collections.Generic;

public class book_of_bishop_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("book_of_bishop_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("book_of_bishop_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                AttackBishopMoves();
                player.ExecuteCurrentCard();
                Debug.Log("Book of Bishop card used: attacked diagonal line positions.");
            }
        }
        else
        {
            Debug.LogError("Card is null in book_of_bishop_card.OnClick");
        }
    }

    private void AttackBishopMoves()
    {
        List<Vector2Int> attackPositions = new List<Vector2Int>();
        Vector2Int playerPosition = player.position;

        // 对角线方向（左上、右上、左下、右下）
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1, -1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(1, 1)
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

public class BookOfBishop : Card
{
    public BookOfBishop() : base(CardType.Special, "S13")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/Book/book_of_bishop");
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/Book/book_of_bishop");
    }
    public override string GetDescription()
    {
        return "攻击对角线方向的所有敌人";
    }
}
