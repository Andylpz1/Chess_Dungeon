using UnityEngine;
using System.Collections.Generic;

public class fan_card : CardButtonBase
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        Debug.Log("fan_card Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected override void OnClick()
    {
        Debug.Log("fan_card OnClick with card: " + (card != null ? card.ToString() : "null"));
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
                PushNearestMonster();
                player.ExecuteCurrentCard();
                Debug.Log("Fan card used: pushed nearest monster two tiles away in a cardinal direction.");
            }
        }
        else
        {
            Debug.LogError("Card is null in fan_card.OnClick");
        }
    }

    private void PushNearestMonster()
    {
        // 使用 monsterManager 查找最近的怪物
        Monster nearestMonster = monsterManager.FindNearestMonster(player.position);
        if (nearestMonster == null)
        {
            Debug.Log("No monster found to push.");
            return;
        }

        // 记录怪物当前的位置，用于后续调试输出
        Vector2Int oldPosition = nearestMonster.position;

        // 计算玩家与怪物之间的差值向量（从玩家指向怪物）
        Vector2Int diff = nearestMonster.position - player.position;
        if (diff == Vector2Int.zero)
        {
            Debug.Log("Monster is at the same position as player, cannot determine push direction.");
            return;
        }

        // 根据横向和纵向的距离选择推送方向（只允许上下左右移动）
        Vector2Int pushDirection = Vector2Int.zero;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            // 横向距离更大
            pushDirection = new Vector2Int(diff.x > 0 ? 1 : -1, 0);
        }
        else if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
        {
            // 纵向距离更大
            pushDirection = new Vector2Int(0, diff.y > 0 ? 1 : -1);
        }
        else
        {
            // 如果相等，优先选择横向
            pushDirection = new Vector2Int(diff.x > 0 ? 1 : -1, 0);
        }

        // 计算一格和两格推送的目标位置
        Vector2Int pushOne = nearestMonster.position + pushDirection;
        Vector2Int pushTwo = nearestMonster.position + pushDirection * 2;

        // 记录所有有效的推送位置
        List<Vector2Int> validPushPositions = new List<Vector2Int>();
        if (player.IsValidPosition(pushOne))
        {
            validPushPositions.Add(pushOne);
            Debug.Log("Valid push position (1 cell): " + pushOne);
        }
        if (player.IsValidPosition(pushTwo))
        {
            validPushPositions.Add(pushTwo);
            Debug.Log("Valid push position (2 cells): " + pushTwo);
        }

        // 优先推送两格，如果两格不可用则尝试推送一格
        if (monsterManager.IsTileValid(pushTwo))
        {
            monsterManager.MoveMonster(nearestMonster, pushTwo);
            Debug.Log("Monster pushed from " + oldPosition + " to " + pushTwo);
        }
        else if (monsterManager.IsTileValid(pushOne))
        {
            monsterManager.MoveMonster(nearestMonster, pushOne);
            Debug.Log("Two-cell push blocked. Monster pushed from " + oldPosition + " to " + pushOne);
        }
        else
        {
            Debug.Log("No valid push positions. Monster not pushed.");
        }
    }

}

public class Fan : Card
{
    public Fan() : base(CardType.Special, "S16")
    {
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Card/Special/fan_card");
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Sprites/Card/Special/fan_card");
    }

    public override string GetDescription()
    {
        return "Fan: Push the nearest enemy two tiles away in a cardinal direction (up, down, left, right).";
    }
}
