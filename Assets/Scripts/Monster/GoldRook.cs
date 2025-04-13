using UnityEngine;
using System.Collections.Generic;

public class GoldRook : Monster
{
    // 定义四个正方向（水平方向和垂直方向）
    private static readonly Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // 右
        new Vector2Int(-1, 0),  // 左
        new Vector2Int(0, 1),   // 上
        new Vector2Int(0, -1)   // 下
    };

    public override void Initialize(Vector2Int startPos)
    {
        base.Initialize(startPos);
        monsterName = "GoldRook";
        // 设置 GoldRook 的类型为 Rook（这里不使用 team 概念，因为所有怪物都是同一阵营）
        type = MonsterType.Rook;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    public override void Die()
    {
        base.Die();
    }

    /// <summary>
    /// GoldRook 的移动逻辑：
    /// 1. 沿上下左右方向扫描所有连续合法且未被占据的格子，
    ///    如果能直接走到玩家所在位置，则直接移动发起攻击；
    /// 2. 否则，对所有候选位置计算评分（评分越低越好）：
    ///      - 基本评分为候选位置到玩家的直线距离；
    ///      - 如果候选位置处于直接攻击线路上（同一行或同一列且路径畅通），给予奖励（减分）；
    ///      - 如果候选位置与玩家距离低于设定的安全距离，则施加惩罚（增加分数），避免“贴脸”。
    /// 3. 当两个候选位置评分非常接近时，会优先选择距离玩家更远的那个（更安全）。
    /// 4. 同时排除那些会阻挡其他直线攻击怪物（例如 Rook 或 Bishop）的候选位置。
    /// </summary>
    public override void MoveTowardsPlayer()
    {
        if (player == null) return;

        lastRelativePosition = position - player.position;
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        // 沿每个方向扫描连续可移动的位置
        foreach (Vector2Int direction in directions)
        {
            Vector2Int candidate = position;
            while (true)
            {
                candidate += direction;
                if (!IsValidPosition(candidate) || IsPositionOccupied(candidate))
                    break;

                // 如果候选位置正好为玩家位置，直接移动进行攻击
                if (candidate == player.position)
                {
                    position = candidate;
                    UpdatePosition();
                    Debug.Log("GoldRook directly attacks the player.");
                    // 此处可加入攻击玩家的逻辑，例如 player.TakeDamage(1);
                    return;
                }
                possibleMoves.Add(candidate);
            }
        }

        // 对候选位置排序：评分越低越好；若评分接近，则优先选择距离玩家更远的
        possibleMoves.Sort((a, b) =>
        {
            float scoreA = EvaluateCandidateMove(a);
            float scoreB = EvaluateCandidateMove(b);
            if (Mathf.Abs(scoreA - scoreB) < 0.001f)
            {
                float distanceA = Vector2Int.Distance(a, player.position);
                float distanceB = Vector2Int.Distance(b, player.position);
                // 距离越大说明越安全，返回值保证排在前面
                return distanceB.CompareTo(distanceA);
            }
            return scoreA.CompareTo(scoreB);
        });

        Vector2Int bestMove = position;
        foreach (Vector2Int move in possibleMoves)
        {
            if (!IsPositionOccupied(move) && IsValidPosition(move) && !WouldBlockFriendlyAttack(move))
            {
                bestMove = move;
                break;
            }
        }

        position = bestMove;
        UpdatePosition();

        if (position == player.position)
        {
            Debug.Log("GoldRook directly attacks the player.");
            //player.TakeDamage(1);
        }
    }

    /// <summary>
    /// 评价候选位置的评分：
    /// - 基本评分为候选位置到玩家的距离；
    /// - 如果候选位置处于直接攻击线路上（水平或垂直且路径畅通），给予奖励（减分）；
    /// - 如果候选位置与玩家距离低于设定的安全距离，则加上惩罚（增加评分）。
    /// 分值越低越优先选择。
    /// </summary>
    private float EvaluateCandidateMove(Vector2Int candidate)
    {
        float baseDistance = Vector2Int.Distance(candidate, player.position);
        float score = baseDistance;

        // 如果候选位置与玩家处于直接攻击线，则给予奖励（扣除 bonus）
        if (IsDirectAttackLine(candidate, player.position))
        {
            float bonus = 5f; // 奖励值，可根据需求调整
            score -= bonus;
        }

        // 安全距离设定（例如 2 格为安全底线）
        float minSafeDistance = 2f;
        if (baseDistance < minSafeDistance)
        {
            // 当距离小于安全距离时，加上惩罚使得评分变高，避免贴脸
            float penalty = (minSafeDistance - baseDistance) * 10f; // 惩罚因子可根据需求调整
            score += penalty;
        }

        return score;
    }

    /// <summary>
    /// 判断 candidate 与玩家是否处于直接进攻线路上（水平或垂直且路径畅通）。
    /// </summary>
    private bool IsDirectAttackLine(Vector2Int from, Vector2Int to)
    {
        if (from.x == to.x || from.y == to.y)
        {
            return IsPathClear(from, to);
        }
        return false;
    }

    /// <summary>
    /// 判断从 from 到 to 的路径是否畅通（中间不受阻），假设两者在同一行或同一列。
    /// </summary>
    private bool IsPathClear(Vector2Int from, Vector2Int to)
    {
        if (from.x == to.x)
        {
            int minY = Mathf.Min(from.y, to.y);
            int maxY = Mathf.Max(from.y, to.y);
            for (int y = minY + 1; y < maxY; y++)
            {
                if (IsPositionOccupied(new Vector2Int(from.x, y)))
                    return false;
            }
            return true;
        }
        else if (from.y == to.y)
        {
            int minX = Mathf.Min(from.x, to.x);
            int maxX = Mathf.Max(from.x, to.x);
            for (int x = minX + 1; x < maxX; x++)
            {
                if (IsPositionOccupied(new Vector2Int(x, from.y)))
                    return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断候选位置是否会阻碍其他直线攻击怪物（如 Rook 或 Bishop）的攻击路径，
    /// 检查该位置是否正好处于其它怪物与玩家之间的直线上。
    /// </summary>
    private bool WouldBlockFriendlyAttack(Vector2Int move)
    {
        Monster[] monsters = FindObjectsOfType<Monster>();
        foreach (Monster m in monsters)
        {
            if (m == null || m == this)
                continue;
            // 只检查依赖直线攻击的怪物类型
            if (m.type == MonsterType.Rook || m.type == MonsterType.Bishop || m.type == MonsterType.Queen)
            {
                if (IsOnLine(m.position, player.position, move))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断点 P 是否在直线 A-B 上（这里仅处理水平或垂直直线），
    /// 且必须位于 A 与 B 之间。
    /// </summary>
    private bool IsOnLine(Vector2Int A, Vector2Int B, Vector2Int P)
    {
        // 水平直线检测
        if (A.y == B.y)
        {
            if (P.y == A.y && (P.x - A.x) * (P.x - B.x) <= 0)
                return true;
        }
        // 垂直直线检测
        if (A.x == B.x)
        {
            if (P.x == A.x && (P.y - A.y) * (P.y - B.y) <= 0)
                return true;
        }
        return false;
    }

    public override GameObject GetPrefab()
    {
        return Resources.Load<GameObject>("Prefabs/Monster/GoldRook");
    }

    /// <summary>
    /// 计算 GoldRook 沿四个方向所有可能的移动位置。
    /// </summary>
    public override List<Vector2Int> CalculatePossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();
        foreach (Vector2Int direction in directions)
        {
            Vector2Int currentPos = position + direction;
            while (IsValidPosition(currentPos) && !IsPositionOccupied(currentPos))
            {
                possibleMoves.Add(currentPos);
                currentPos += direction;
            }
        }
        return possibleMoves;
    }
}
