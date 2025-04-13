using UnityEngine;
using System.Collections.Generic;

public class GoldPawn : Monster
{
    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        type = MonsterType.Pawn;
        monsterName = "GoldPawn";
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
    }

    public override void MoveTowardsPlayer()
    {
        if (player == null) return;

        // 记录相对玩家的位置
        lastRelativePosition = position - player.position;

        // 目标位置直接设为玩家的位置
        Vector2Int targetPosition = player.position;
        Debug.Log("GoldPawn: Moving towards player at " + targetPosition);

        // 构建候选移动方向（8 个方向：上下左右及四个对角方向）
        List<Vector2Int> possibleMoves = new List<Vector2Int>()
        {
            new Vector2Int(position.x + 1, position.y),      // 右
            new Vector2Int(position.x - 1, position.y),      // 左
            new Vector2Int(position.x, position.y + 1),      // 上
            new Vector2Int(position.x, position.y - 1),      // 下
            //new Vector2Int(position.x + 1, position.y + 1),  // 右上
            //new Vector2Int(position.x - 1, position.y + 1),  // 左上
            //new Vector2Int(position.x + 1, position.y - 1),  // 右下
            //new Vector2Int(position.x - 1, position.y - 1)   // 左下
        };

        // 根据候选移动位置到玩家位置的距离排序，距离越近越优先
        possibleMoves.Sort((a, b) => Vector2Int.Distance(a, targetPosition)
                                    .CompareTo(Vector2Int.Distance(b, targetPosition)));

        // 遍历候选方向，选择第一个既不被占用、位置合法且不会阻挡友军攻击路线的移动
        foreach (Vector2Int move in possibleMoves)
        {
             if (move == player.position)
        {
            position = move;
            UpdatePosition();
            break;
        }
            if (!IsPositionOccupied(move) && IsValidPosition(move) && !WouldBlockFriendlyAttack(move))
            {
                position = move;
                UpdatePosition();
                break;
            }
        }

        // 若移动后与玩家重合，则触发攻击
        if (position == player.position)
        {
            Debug.Log("Player attacked by GoldPawn.");
            // 可在此加入攻击逻辑，例如 player.TakeDamage(1);
        }
    }


    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/GoldPawn");
    }

    /// <summary>
    /// 尝试寻找附近的高价值目标（如女王、车、象等）作为移动目标。
    /// 该方法的具体实现需要依赖你的棋盘布局和棋子评估逻辑。
    /// </summary>
    /// <param name="targetPos">
    /// 如果找到高价值目标，则通过 out 参数返回目标位置；
    /// 否则默认使用玩家的位置。
    /// </param>
    /// <returns>如果找到了高价值目标，返回 true；否则返回 false</returns>
    private bool TryGetHighValueTarget(out Vector2Int targetPos)
    {
        float bestValue = -1f;
        targetPos = player.position; // 默认目标为玩家位置
        bool foundTarget = false;
        // 定义威胁范围，例如离玩家 3 格以内就认为该目标处于玩家攻击范围内
        float threatRange = 3f;

        // 遍历所有 Monster 对象（所有怪物都是敌方，但它们彼此为盟友）
        Monster[] monsters = FindObjectsOfType<Monster>();
        foreach (Monster m in monsters)
        {
            if (m == null || m == this) // 排除空对象和自身
                continue;

            // 计算该怪物与玩家之间的距离
            float distance = Vector2Int.Distance(player.position, m.position);
            // 只有处于玩家威胁范围内的怪物才被考虑，同时选择数值最高的
            if (distance <= threatRange && m.pieceValue > bestValue)
            {
                bestValue = m.pieceValue;
                targetPos = m.position;
                foundTarget = true;
            }
        }
        return foundTarget;
    }

    /// <summary>
    /// 判断移动到指定位置 move 是否会阻挡己方重要怪物（如车或象）的攻击路线，
    /// 这里假定己方怪物的攻击目标为 player。
    /// </summary>
    /// <param name="move">候选移动位置</param>
    /// <returns>如果该位置会阻挡友军攻击返回 true，否则返回 false</returns>
    private bool WouldBlockFriendlyAttack(Vector2Int move)
    {
        // 遍历所有己方怪物
        Monster[] monsters = FindObjectsOfType<Monster>();
        foreach (Monster m in monsters)
        {
            if (m == null)
                continue;
            //排除 GoldPawn 自己
            if (m == this)
                continue;
            // 只检查类型为车或象的怪物
            if (m.type == MonsterType.Rook || m.type == MonsterType.Bishop)
            {
                Vector2Int friendlyPos = m.position;
                Vector2Int targetPos = player.position; // 假定这些怪物的攻击目标为玩家
                if (IsOnLine(friendlyPos, targetPos, move))
                {
                    // 如果候选位置位于友军怪物与玩家之间，则可能阻挡它们的攻击路线
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 判断点 P 是否在点 A 与点 B 的直线上，并且位于 A 与 B 之间（包括边界），
        /// 支持水平、垂直以及对角线方向。
    /// </summary>
    // <param name="A">直线起点</param>
    /// <param name="B">直线终点</param>
    /// <param name="P">待检测的点</param>
    /// <returns>如果 P 在 A 和 B 之间返回 true，否则返回 false</returns>
    private bool IsOnLine(Vector2Int A, Vector2Int B, Vector2Int P)
    {
        // 检查水平直线
        if (A.y == B.y)
        {
            if (P.y == A.y && (P.x - A.x) * (P.x - B.x) <= 0)
                return true;
        }
        // 检查垂直直线
        if (A.x == B.x)
        {
            if (P.x == A.x && (P.y - A.y) * (P.y - B.y) <= 0)
                return true;
        }
        // 检查对角线
        int dx = B.x - A.x;
        int dy = B.y - A.y;
        if (Mathf.Abs(dx) == Mathf.Abs(dy))
        {
            if (Mathf.Abs(P.x - A.x) == Mathf.Abs(P.y - A.y))
            {
                if ((P.x - A.x) * (P.x - B.x) <= 0 && (P.y - A.y) * (P.y - B.y) <= 0)
                    return true;
            }
        }   
        return false;
    }

}
